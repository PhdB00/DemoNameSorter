# Name Sorter

A Coding Assessment: A .NET application that sorts names from a file by Last Name and then by Given Names.

## Overview

This was a coding assessment that asked to show an interesting solution to an extremely simple problem. The approach taken here was clearly excessive for such a simple problem but was an entertaining way of applying software development techniques for demonstration purposes only.

Name Sorter is a command-line application that reads a list of names from a file, sorts them according to specific rules, and outputs the sorted results both to the console and to a file.

## Features

- Reads names from an input file
- Supports names with 1 to 3 given names and a single last name
- Sorts names by last name, then by given names
- Outputs sorted names to both console and file
- Flexible pipeline architecture for easy extension
- Command-line interface with customizable file input and output location

## Requirements

- .NET 9.0
- Windows/Linux/macOS compatible

## Installation

1. Clone the repository
2. Build the solution:
```bash
dotnet build
```

## Usage

Basic usage:
```bash
name-sorter ./unsorted-names-list.txt
```

With custom output file:
```bash
name-sorter ./unsorted-names-list.txt --output ./custom-output.txt
```

### Input File Format

- One name per line
- Each name can have between 1 and 3 given names followed by a single last name
- Names must be separated by single spaces

Example input file:

```txt
John Coltrane 
Peter Brotzmann
Tim Jones 
Wolfgang Amadeus Mozart
```

### Output

The application will:
1. Read names from the input file
2. Sort them according to the rules
3. Display the sorted names on the console
4. Write the sorted names to 'sorted-names-list.txt' (or to another output file specified in the command-line)

## Architecture

The application uses a pipeline-based architecture with the following key components:

### Core Components

- **Pipeline Pattern**: Modular processing steps for extensibility
- **Dependency Injection**: Using Microsoft.Extensions.DependencyInjection
- **Command Line Interface**: Using System.CommandLine
- **Strategy Pattern**: For flexible output handling

### Pipeline Steps

1. **Read**: Extracts names from input file
2. **Sort**: Orders names by last name and given names
3. **Output**: Writes results to console and file

### Key Interfaces

- `IPipelineStep`: Base interface for all pipeline steps
- `IOutputStrategy`: Defines output behavior
- `INameParser`: Handles name parsing logic
- `INameSorter`: Manages name sorting functionality

## Error Handling

The application includes comprehensive error handling for:
- Invalid input files
- Malformed names
- Invalid command-line arguments
- File system issues

However as requirements develop we may prefer to provide more meaningful feedback to end-users about issues, especially with the content of the supplied input file 

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

MIT License

Copyright (c) 2025 PB

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Contact

Please use GitHub Issues for all communications regarding this project.
