![Splash Screen](IFME/Resources/SplashScreen14.png)

# Internet Friendly Media Encoder
![GUI](IFME.png)

**Language:** [English](/README.md)

## 紹介
2012 年、当時大学生だった私は FRAPS のゲーム録画を x264 で圧縮してアーカイブにするために IFME を開発しました。友人たちは IFME のシンプルで軽量な設計を高く評価したことで Internet Friendly Media Encoder (IFME) の誕生に繋がりました。

## Internet Friendly Media Encoder (IFME) ついて
使いやすさを重視して設定された多用途で将来性があり、拡張性に優れたマルチメディアエンコーダーです。Internet Friendly Media Encoder は、字幕や添付ファイルの追加に対応。その他に「ストリームのコピー」のオプションを備えた多重化ツールとしても機能します。複数のビデオとオーディオ、字幕、添付ファイルのストリームを 1 つのファイルに結合や不要なストリームを削除、別のビデオから字幕を抽出せずに組み込みができます。また、高度なビデオ処理を可能にする AviSynth にも対応しています。

ビデオおよびオーディオエンコーダーはプラグイン形式で実装されており、ユーザーは独自のコンパイルされた CPU アーキテクチャを追加することで速度を向上させることができます。<br>
このモジュール設計により、エンコーダーの適応性と拡張性が維持され、新しいエンコーディング技術や将来のエンコーディング技術を追加することができます。

## ライセンス
### ソースコードとバイナリについて
IFME のソースコードは [GPL 2.0](http://choosealicense.com/licenses/gpl-2.0/)
 に基づいています。
### マスコットとアートワークについて
[53C](http://53c.deviantart.com/) と [adeq](https://www.facebook.com/liyana.0426) によって描かれたアートワークは IFME プロジェクトの所有物です。<br>
[表示-非営利 4.0 国際](https://creativecommons.org/licenses/by-nc/4.0/deed.ja)の CC ライセンスに基づいています。

## 寄付
このプロジェクトを応援してください！<br>
ほんの少々の寄付でもこのプロジェクトの活性化と今後の維持に繋がります。

[PayPal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4CKYN7X3DGA7U) から寄付が行えます。寄付をしてくれた方は、[Facebook](https://www.facebook.com/internetfriendlymediaencoder) または [Twitter](https://twitter.com/Anime4000) から報告してください。<br>
表彰と殿堂入りおよび、プログラムについてに掲載されます。

## あなたへ
### システム要件
* [Microsoft Visual C++ (すべて)](https://www.techpowerup.com/download/visual-c-redistributable-runtime-package-all-in-one/)
* [Microsoft .NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48) *<sup>Windows 7 のみ</sup>*

Ubuntu (Linux):
* `mono-complete` (Mono ランタイム)
* *技術的には、IFME は Linux でも動作しますが`プラグイン`フォルダーは Windows 専用です。`FFmpeg`、`x264`、`x265` などの独自バージョンをビルドして Windows バイナリを置き換える必要があるかもしれません。*

#### 32 ビットについて
*IFME は高解像度 (QHD、HUD) および高ビット深度で非常に大容量な RAM を要するため、32 ビットのサポートを終了しました*

#### 64 ビット
* OS: 64 ビットの Windows 10 と Linux (Ubuntu など...)
* CPU: Intel Core i9 10 または AMD Ryzen 7 3700X
* RAM:  8GB DDR4 (QHD とそれ以下)
* RAM: 16GB DDR4 (UHD とそれ以上)
* HDD: 70GB (UHD、HDR 一時ディスク)

## ダウンロード
こちらからダウンロード @
* [SourceForge (リリース)](https://sourceforge.net/projects/ifme/files/latest/download)
* [GitHub (リリース)](https://github.com/Anime4000/IFME/releases/latest)
* [VideoHelp](https://www.videohelp.com/software/Internet-Friendly-Media-Encoder)
* [SoftPedia](https://www.softpedia.com/get/Multimedia/Video/Encoders-Converter-DIVX-Related/Internet-Friendly-Media-Encoder.shtml)

> [!NOTE]
> VideoHelp と SoftPedia は私が管理していないため、最新のバージョンになるまでに時間がかかる可能性があります。SourceForge は常に最新のバージョンであり、GitHub はリリースをし忘れてしまうことがあります。

### 実行
Windows のユーザーは `ifme.exe` を実行するだけですが、Linux のユーザーはターミナルエミュレーター経由で `ifme.sh` を実行する必要があります。

## ハードウェアアクセラレーション
Internet Friendly Media Encoder には [Rigaya](https://github.com/rigaya) を使用した H264、H265、AV1 用のハードウェアアクセラレーションが組み込まれています。

> [!WARNING]
> 高効率なエンコード (高画質+低ビットレート) には、CPU ベースのエンコーダーの使用を推奨します。ハードウェアアクセラレーションを使用するとエンコード速度は向上しますが、画質とビットレート効率が低下する可能性があります。

## 開発者向け
このプロジェクトをクローンして VisualStudio 2022 を使用して `ifme.sln` を開きます。既定では、VS 2022 は nuget から `Newtonsoft.Json` を自動でダウンロードします。
