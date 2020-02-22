#!/bin/bash

exit -1

RESULT=$(mongo ./dev_prepmongo.js)

if [ $? -ne 0 ]; then
        echo "dev_prepmongo.js failed" 1>&2
        echo $RESULT 1>&2
fi

exit $?
