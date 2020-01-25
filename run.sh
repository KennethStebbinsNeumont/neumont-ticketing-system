#!/bin/bash

# Determine which scripts to run
mode=$1
if [[ "$mode" != "dev" && "$mode" != "prod" ]]; then
    echo "Usage: $0 <dev|prod> [search_dir]"
    exit 1
fi

# Determine searching directory
searchdir=$2
if [ "$2" == "" ]; then
        searchdir=$(pwd)
fi
searchdir_len=${#searchdir}
let "index = searchdir_len - 1"
lastchar=${searchdir:$index:1}
if [ "$lastchar" == "/" ]; then
        searchdir=${searchdir:0:$index}
fi

files=$(ls $searchdir | grep -e ^${mode}_.*\.sh)

if [ "$files" != "" ]; then
        while read file; do
                /bin/bash "$searchdir/$file"
        done <<< $files
fi

