#!/bin/sh
path="$(dirname $(readlink -f $0))"
export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:"$path"
exec "$path/ifme-bin" "$@"