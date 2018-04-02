import sys
import ast
import astor

file = sys.argv[1]
tstring = sys.argv[2] #type
name = sys.argv[3] #name

if tstring == 'def':
    ttype = ast.FunctionDef
elif tstring == 'class':
    ttype = ast.ClassDef
else:
    ttype = ast.FunctionDef
    
tree = astor.parse_file(file)

def search(ast):
    
    if type(ast) == ttype  and ast.name == name:
        return ast

    if hasattr(ast, 'body'):
        for item in ast.body:
            result = search(item)
            if result != None:
                return result
    
    return None

r = search(tree)

if r != None:
    print astor.to_source(r)
else:
    print "-1"




