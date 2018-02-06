var recast = require("recast");
var fs = require('fs');
// print process.argv

// file1 and file2
let file1 = process.argv[2];
let type = process.argv[3];
let name = process.argv[4];

if (type == 'function')
    type = 'FunctionDeclaration';

// read the first file
fs.readFile(file1, 'utf8', function (err, fileData1) {
    if (err) {
        console.log(err);
        return;
    }

    // compare that two files then write results to screen
    console.log(print(fileData1));
});

// a regular tree search
function search(ast, type, name) {

    for (let i = 0; i < ast.body.length; i++) {
        if (ast.body[i].type === type && ast.body[i].id.name === name)
            return ast.body[i];
        else {
            if (ast.body[i].body !== undefined)
                return search(ast.body[i].body, type, name);
        }
    }

    return null;
}

function print(data1) {
    let ast = recast.parse(data1);

    var node = search(ast.program, type, name);

    if (node !== null) {
        return recast.print(node).code;
    }
    else
        return "";
}

