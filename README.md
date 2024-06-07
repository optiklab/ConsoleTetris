# ConsoleTetris

|Branch      |Status   |
|------------|---------|
|master      | [![Build Status](https://api.travis-ci.com/optiklab/ConsoleTetris.svg?branch=main)](https://api.travis-ci.com/optiklab/ConsoleTetris.svg?branch=main) |

## Play?

<p align="center">
	You can play this game in your browser:
	<br />
	<a href="https://optiklab.github.io/ConsoleTetris" alt="Play Now">
		<sub><img height="40"src=".github/resources/play-badge.svg" alt="Play Now"></sub>
	</a>
	<br />
	<sup>Hosted On GitHub Pages</sup>
</p>

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
