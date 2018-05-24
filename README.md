# Triggr

This is my ITU graduation project.

Triggr allows  software  developers  define  probes  as
JSON files for monitoring code changes over the main shared codebase (e.g. Git) or calculating metrics for a selected piece of code. It also supports sending customized feedback to the developers  through  pre-specified  channels.  The  user-defined probes  are  triggered  through  continuous  monitoring  of  the shared code repository rather than being dependent on a build pipeline of a CI server.

## How to ?

You have to download .NET Core from [download](https://www.microsoft.com/net/download/)

Also, it would be nice to have [VSCode](https://code.visualstudio.com/download).

Follow the instructions
```
git clone https://github.com/lyzerk/Triggr.git
cd Triggr
dotnet restore
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