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
else
    owd=$(pwd)
    cd $searchdir
fi

files=$(ls | grep -e ^${mode}_.*\.sh)

if [ "$files" != "" ]; then
    while read file; do
        /bin/bash "$file"
        if [ $? -ne 0 ]; then
            # If this script failed
            echo "$file exited with non-zero exit status." 1>&2
            exit $?
        fi
    done <<< $files
fi

if [ "$owd" != "" ]; then
    cd "$owd"
fi