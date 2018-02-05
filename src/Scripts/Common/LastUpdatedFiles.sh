#!/bin/bash
gitIndex=0
cd $1 && git show HEAD --name-only | \
while read i
do
    # ignore first 6 lines
    if [ "$gitIndex" -ge 6 ]; then
        echo $i
    fi
    ((gitIndex++))
done