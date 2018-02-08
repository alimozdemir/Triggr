if [[ -z "$2" ]]; then
   pylint $1
elif [[ -z "$3" ]]; then
   pylint $1 $2
elif [[ -z "$4" ]]; then
   pylint $1 $2 $3
elif [[ -z "$5" ]]; then
   pylint $1 $2 $3 $4
elif [[ -z "$6" ]]; then
   pylint $1 $2 $3 $4 $5
else
    echo "too much parameters" $1 $2 $3 $4 $5
fi
