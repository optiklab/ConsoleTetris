# ConsoleTetris

|Branch      |Status   |
|------------|---------|
|master      | [![Build Status](https://api.travis-ci.com/optiklab/ConsoleTetris.svg?branch=main)](https://api.travis-ci.com/optiklab/ConsoleTetris.svg?branch=main) |

## What?

Simple, NOT efficient (:D), console tetris written in C# .NET Core and works on Windows, Linux and Mac in the console/terminal.
I do not know how Tetris SHOULD be implemented, but I spent couple of evenings to make this from scratch for fun.




## Why not efficient?

Because I can >_<

Well, first of all, it's written with standard C# collections, instead of any kind of matrix logic. Just for simplisity of understanding.
Secondly, it sometimes uses Linq, which is even slower from time to time.

It doesn't use any kind of buffering (maybe I will play more with it and add it, but in this case I might have to rewrite everthing... so I just lazy to do it).
It uses 2nd thread to track user input, so I have Locks to access commands collection.

So, overall, not very well written, but it works. And it makes some fun.

## How it looks like?

```bash
------------------
|                |
|       **       |
|        **      |
|                |
|                |
|                |
|                |
|                |
|                |
|                |
|                |
|                |
|       *        |
|** *   *  ****  |
|*************** |
-***************--
Points earned: 200
```

## How to run?

1. Make sure .NET Core 3.0 is installed on your Machine (any OS).
   You can download it from here https://dotnet.microsoft.com/download.
   
   In case you installed some upper versions than 3.0, you will need a really minor change in *src/ConsoleTetris.csproj* in this line:
```bash
<TargetFramework>netcoreapp3.0</TargetFramework>
```
    See the appropriate values https://docs.microsoft.com/en-us/dotnet/standard/frameworks.    

2. Execute commands:
```bash
$> dotnet build
$> dotnet run
```

## How to play?

* Left & Right Arrows - move left & right
* Down Arrow - drop with speed
* Space - Rotate
* E - exit
* R - reset

## Is there end/win?

I wasn't so thoughtful about this prototype. There are no WIN in the end.


## License

I'm not sure why you would be interested in License, but in case you do - feel free to use this code in any way you need.