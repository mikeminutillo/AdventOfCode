# Advent of Code

Repository for my solutions to the [Advent of Code](https://adventofcode.com/) programming challenges.

There is a test in [`Helpers.cs`](/Helpers.cs) which sets up each day and grabs your unique input for you.

As we are not supposed to share inputs, these are stored in a different folder. The path to this folder is stored in an environment variable named `ADVENT_OF_CODE_INPUT_PATH`.

There is another environment variable named `ADVENT_OF_CODE_SESSION_KEY`. If this contains the value of your `session` cookie (when logged in) on the Advent of Code website then the helper code can grab inputs for you.

Each days solution goes in a class that inherits from [`AdventOfCodeBase<T>`](/AdventOfCodeBase.cs). This abstract base class configures all of the tests for you and provides virtual methods for `Solution1` and `Solution2`. Override these and return the result. The infrastructure will run every input over them and store the result in a `.received.txt` file named after the input and the part of the problem (one or two). It will then compare this to a `.approved.txt` file with the same prefix. If they are the same the test passes.

Solve the problem. When you have the right result, copy the contents of the `.received.txt` file into the `.approved.txt` file. Now, subsequent runs will pass.