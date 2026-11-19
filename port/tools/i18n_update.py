#!/usr/bin/env python3
"""i18n_update.py - keep IFME 9's translation files in step with the code.

    python tools/i18n_update.py [--seed ../IFME/i18n] [--new xx-YY] [--report]

Run from port/. It reads every tr("...") / QObject::tr("...") in gui/*.cpp and
gui/*.h and updates i18n/<language>.json:

  - new strings are added with an empty translation (shown in English until
    someone translates them), in the order they appear in the code;
  - translations of strings the code no longer has move to "Obsolete", so
    nothing a translator wrote is lost;
  - --seed DIR fills empty translations from IFME 8's i18n/<language>.json
    wherever IFME 8 has the same English text (ignoring '&', a trailing ':'
    or '...', and case), and copies its author details into new files;
  - --new xx-YY starts a file for another language;
  - --report prints how much of each language is translated.

File format (UTF-8):
    { "Language": "ja-JP", "AuthorName": "", "AuthorProfile": "", "AuthorEmail": "",
      "Strings": { "English text": "translation", ... }, "Obsolete": { ... },
      "Drafts": [ "English text", ... ] }

"Drafts" lists machine-drafted translations nobody has reviewed yet; a
translator removes a string from it once it is checked.
"""
import argparse
import glob
import json
import os
import re
import sys

TR = re.compile(r'\b(?:QObject::|QCoreApplication::)?tr\(\s*((?:"(?:[^"\\]|\\.)*"\s*)+)')
LITERAL = re.compile(r'"((?:[^"\\]|\\.)*)"')
ESCAPES = {'n': '\n', 't': '\t', '"': '"', '\\': '\\', "'": "'", 'r': '\r', '0': '\0'}


def unescape(s):
    return re.sub(r'\\(.)', lambda m: ESCAPES.get(m.group(1), m.group(1)), s)


def extract(files):
    seen, out = set(), []
    for path in files:
        text = open(path, encoding='utf-8').read()
        for m in TR.finditer(text):
            s = ''.join(unescape(x) for x in LITERAL.findall(m.group(1)))
            if s and s not in seen:
                seen.add(s)
                out.append(s)
    return out


def norm(s):
    s = re.sub(r'\(&\w\)', '', s).replace('&', '')
    s = s.strip().rstrip(':：').rstrip('.…').strip()
    return re.sub(r'\s+', ' ', s).lower()


def flatten(o, prefix=''):
    """IFME 8's i18n JSON -> {key path: text} for every string leaf."""
    out = {}
    if isinstance(o, dict):
        for k, v in o.items():
            out.update(flatten(v, f'{prefix}/{k}'))
    elif isinstance(o, str):
        out[prefix] = o
    return out


def cjk(lang):
    return lang.split('-')[0] in ('ja', 'zh', 'ko')


def adapt(source, translation, lang):
    """Give IFME 8's translation the source's mnemonic and trailing colon."""
    t = translation.strip()
    has_colon = source.rstrip().endswith(':')
    t_colon = t.endswith(':') or t.endswith('：')
    if has_colon and not t_colon:
        t += '：' if cjk(lang) and translation.strip().endswith('）') else ':'
    elif not has_colon and t_colon:
        t = t.rstrip(':：')
    letter = re.search(r'&(\w)', source)
    if '&' not in source:
        t = re.sub(r'\(&\w\)', '', t).replace('&', '')
    elif letter and cjk(lang) and re.search(r'\(&\w\)', t):
        # "(&X)" after the word, as CJK menus do, with this source's key.
        t = re.sub(r'\(&\w\)', f'(&{letter.group(1).upper()})', t)
    elif '&' not in t:
        letter = re.search(r'&(\w)', source)
        if letter and cjk(lang):
            colon = t[-1] if t[-1] in ':：' else ''
            t = (t[:-1] if colon else t) + f'(&{letter.group(1).upper()})' + colon
    return t


def seed_table(seed_dir, lang):
    en = os.path.join(seed_dir, 'en-US.json')
    tr = os.path.join(seed_dir, f'{lang}.json')
    if not (os.path.exists(en) and os.path.exists(tr)):
        return {}, {}
    en_flat = flatten(json.load(open(en, encoding='utf-8-sig')))
    tr_json = json.load(open(tr, encoding='utf-8-sig'))
    tr_flat = flatten(tr_json)
    table = {}
    for key, english in en_flat.items():
        t = tr_flat.get(key)
        if t and t != english and norm(english) and norm(english) not in table:
            table[norm(english)] = t
    author = {k: tr_json.get(k, '') for k in ('AuthorName', 'AuthorProfile', 'AuthorEmail')}
    return table, author


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument('--seed', help="IFME 8's i18n folder, to fill empty translations from")
    ap.add_argument('--new', action='append', default=[], help='start a file for this language (e.g. ko-KR)')
    ap.add_argument('--report', action='store_true')
    args = ap.parse_args()

    strings = extract(sorted(glob.glob('gui/*.cpp')) + sorted(glob.glob('gui/*.h')))
    os.makedirs('i18n', exist_ok=True)
    files = {os.path.splitext(os.path.basename(p))[0]: p for p in glob.glob('i18n/*.json')}
    for lang in args.new:
        files.setdefault(lang, os.path.join('i18n', f'{lang}.json'))

    for lang, path in sorted(files.items()):
        data = json.load(open(path, encoding='utf-8-sig')) if os.path.exists(path) else {}
        old = dict(data.get('Strings', {}))
        obsolete = dict(data.get('Obsolete', {}))
        table, author = seed_table(args.seed, lang) if args.seed else ({}, {})

        new = {}
        for s in strings:
            t = old.pop(s, None) or obsolete.pop(s, None) or ''
            if not t and table.get(norm(s)):
                t = adapt(s, table[norm(s)], lang)
            new[s] = t
        obsolete.update({k: v for k, v in old.items() if v})

        out = {'Language': lang}
        for k in ('AuthorName', 'AuthorProfile', 'AuthorEmail'):
            out[k] = data.get(k) or author.get(k, '')
        out['Strings'] = new
        if obsolete:
            out['Obsolete'] = obsolete
        drafts = [s for s in new if new[s] and s in set(data.get('Drafts', []))]
        if drafts:
            out['Drafts'] = drafts
            out['DraftNote'] = data.get('DraftNote', '')
        with open(path, 'w', encoding='utf-8', newline='\n') as f:
            json.dump(out, f, ensure_ascii=False, indent='\t')
            f.write('\n')
        done = sum(1 for v in new.values() if v)
        print(f'{lang}: {done}/{len(new)} translated' + (f' ({len(drafts)} drafts)' if drafts else '')
              + (f', {len(obsolete)} obsolete' if obsolete else ''))

    if args.report and not files:
        print(f'{len(strings)} strings, no translation files yet (use --new xx-YY)')
    return 0


if __name__ == '__main__':
    sys.exit(main())
