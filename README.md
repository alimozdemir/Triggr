# triggr

ITU Graduation project

## How to run

You have to download .NET Core from [download](https://www.microsoft.com/net/download/)

Also, it would be nice to have [VSCode](https://code.visualstudio.com/download).

Follow the instructions
```
git clone https://github.com/lyzerk/Triggr.git
cd Triggr
dotnet restore src/Triggr.UI/Triggr.UI.csproj
```

If you do have VSCode.

```
code .
```

It will offer you the restoring process. Just accept it. 

Press F5. Then you are ready to go.

If you don't have VSCode

```
dotnet run --project src/Triggr.UI/Triggr.UI.csproj
```

I couldn't test it yet, but maybe you have to delete .db files for different OS's than macOS. It will generate the database files again for your OS.