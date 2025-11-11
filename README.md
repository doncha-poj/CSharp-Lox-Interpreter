# A Lox Interpreter Written in C#
This was made for my class to create a lox interpreter using this textbook: https://craftinginterpreters.com/

Any language could be chosen to write this interpreter (with the exception of Java because the website uses that language)

It's encouraged to use a langauge you do not know to write this; I used C#.

## Prerequisites

Before building, you must have the **.NET 8.0 SDK** installed.
* [.NET 8.0 SDK Download](https://dotnet.microsoft.com/download/dotnet/8.0)

This project is configured to build as a .NET 8.0 console application.

---

## Option 1: Using Visual Studio 2022

This is the simplest, all-in-one method.

1.  **Install Visual Studio:** Make sure you have **Visual Studio 2022** installed with the **".NET desktop development"** workload.
2.  **Open Solution:** Double-click the `cslox.sln` file to open the entire solution.
3.  **Run:** Press the **Start button (F5)** or use the `Ctrl+F5` shortcut (Start Without Debugging).
4.  **Result:** The project will compile, and the console window will open, running the interpreter in its interactive REPL mode.

---

## Option 2: Using Visual Studio Code

This method is ideal for cross-platform development (Windows, macOS, Linux) and relies on the .NET command-line interface (CLI).

1.  **Install VS Code:** Make sure you have **Visual Studio Code** installed.
2.  **Install Extension:** Open VS Code, go to the Extensions tab, and install the **"C# Dev Kit"** from Microsoft.
3.  **Open Folder:** In VS Code, go to **File > Open Folder...** and select the main `CSharp-Lox-Interpreter` folder (the one containing `cslox.sln`).
4.  **Open Terminal:** Open the integrated terminal in VS Code (**View > Terminal** or `Ctrl+~`).
5.  **Run Project:** You can now run the interpreter directly from the terminal.

---

## How to Use the Interpreter (via Terminal)

Once the .NET SDK is installed, you can run the interpreter from any terminal.

### 1. REPL Mode (Interactive Prompt)

To compile and run the interpreter in its interactive REPL mode, run the `dotnet run` command from the root folder. This is the default mode when no arguments are given.

```bash
dotnet run --project cslox/cslox.csproj
```

**Note**: You can exit the terminal with CTRL+C when you are done.

You will see the `>` prompt. You can then type Lox code, one line at a time:

> print "Hello, world!";
Hello, world!
> var a = 10;
> print a * 2;
20

**Note**: A file called test-repl.txt has been provided to test each chapter.

### 2. To execute a Lox script file, provide the path to the file as an argument.

For example, if you have a file named test-file.lox:

`dotnet run --project cslox/cslox.csproj "test-file.lox"`

The interpreter will execute the contents of test.lox and print any output.
