if [[ -z "$3" ]]; then
   eslint $1 --parser-options "ecmaVersion:6" --rule "$2"
elif [[ -z "$4" ]]; then
   eslint $1 --parser-options "ecmaVersion:6" --rule "$2" --rule "$3"
elif [[ -z "$5" ]]; then
   eslint $1 --parser-options "ecmaVersion:6" --rule "$2" --rule "$3" --rule "$4"
elif [[ -z "$6" ]]; then
   eslint $1 --parser-options "ecmaVersion:6" --rule "$2" --rule "$3" --rule "$4" --rule "$5"
else
    echo "-1"
fi
