# 🤝 Contributing to NeoBleeper

First off, thank you for considering contributing to NeoBleeper! Your contributions are integral to the success of this project. Whether you're reporting a bug, proposing a feature, improving documentation, uploading legacy BMM file or NBPML file, or submitting code, your involvement is highly valued.

## 📑 Table of Contents
1. [Code of Conduct](#code-of-conduct)
2. [How Can I Contribute?](#how-can-i-contribute)
   - [Bug Reports](#bug-reports)
   - [Feature Requests](#feature-requests)
   - [Code Contributions](#code-contributions)
   - [Documentation](#documentation)
   - [BMM and NBPML File Contributions](#bmm-and-nbpml-file-contributions)
3. [Pull Request Process](#pull-request-process)
4. [Style Guides](#style-guides)
   - [Code Style](#code-style)
   - [C# Specific Notes](#c-sharp-specific-notes)
5. [Community Support](#community-support)

## 🌟 Code of Conduct
By participating in this project, you agree to abide by the Code of Conduct. Please be respectful and considerate to others in the community. See the `CODE_OF_CONDUCT.md` file for details.

## 🤝🙋‍♂️ How Can I Contribute?

### 🪲 Bug Reports
If you've found a bug in NeoBleeper, please create an issue and include the following details:
- A clear and descriptive title.
- The version of NeoBleeper or commit hash if applicable.
- Steps to reproduce the issue, or a code snippet.
- Expected and actual behavior.
- Any other relevant details, including screenshots or crash logs.

### 💭 Feature Requests
We welcome your ideas! To request a feature:
1. Check the issues to see if someone else has already requested it.
2. If not, open a new issue, and share a detailed description including:
   - Background for the request.
   - Why it is valuable.
   - Potential impacts, risks, or considerations.

### 👩‍💻 Code Contributions
1. Fork the repository and create a new branch off `main`. Name your branch something descriptive, such as `feature/add-tune-filter`.
2. Open the repository folder in Visual Studio:
   - Make sure you have [Visual Studio](https://visualstudio.microsoft.com/) installed with the required workloads (e.g., ".NET desktop development" for NeoBleeper).
   - Clone your fork of the repository to your local machine (you can use Visual Studio's integrated Git tools or the Git CLI).
   - Once cloned, open the solution (`.sln`) file in Visual Studio.
3. Install NuGet Packages:
   - Restore any required dependencies by clicking on `Restore NuGet Packages` in the top bar or running `dotnet restore` from the terminal.
4. Add your changes:
   - Use Visual Studio's features such as IntelliSense, debugging, and code formatting to contribute effectively.
   - Ensure proper tests are included and all existing tests pass.
   - Make sure your code adheres to the style guide.
5. Add your name or nickname to the About Page:
   - Open the `about_neobleeper.cs` file and locate the `listView1` component.
   - Select the `listView1` component in the Visual Studio designer.
   - Click the small arrow in the upper-right corner of the component to open the dropdown menu.
   - Select **Edit Items** to open the ListView items collection editor.
   - Add a new `ListViewItem`:
      - Write your name or nickname in the **Text** property.
      - For your contributions/tasks:
         - Locate the **SubItems** property.
         - Click the triple dots (`...`) to the right of the `(Collection)` field.
         - Add or edit the **SubItem** with a short description of your tasks.
   - If you've already added your name, edit the SubItem or update your existing entry before committing your changes.
6. Test your code:
   - Run the tests using Visual Studio's Test Explorer.
   - Fix any failing tests and validate your changes.
7. Commit your changes with clear and concise messages.
   - Use Visual Studio’s integrated Git tools to stage and commit your changes.
8. Push your branch and open a pull request in the repository.
9. Be prepared to work with reviewers and revise as needed.
    
### 🧾 Documentation
Improving our documentation is one of the easiest ways to contribute! Feel free to add or update examples, clarify sections, or enhance overall readability.

### 🎼 BMM and NBPML File Contributions
NeoBleeper supports legacy BMM (Bleeper Music Maker) and NBPML (NeoBleeper Project Markup Language) files. If you're contributing or working with these file types, ensure the following:
- Validate that BMM files are parsed correctly and render as expected in NeoBleeper.
- Test compatibility with both legacy formats and the current implementation.
- For NBPML files, maintain adherence to the latest NeoBleeper Project Markup Language specifications.

If you encounter any issues specific to these file formats, please follow the guidelines in the "Bug Reports" section. Feature requests for enhanced support for BMM and NBPML files are also welcome!

## ⬇️ Pull Request Process
All submissions should be made via pull requests. Here’s the process:
1. Fill out the pull request template.
2. Ensure your pull request does not duplicate existing ones.
3. Add the details about your changes in the description, referencing related issues where possible.
4. Address all comments or requested changes from reviewers.
5. Pull requests must pass all CI/CD checks, including tests and code quality checks.

## 📖 Style Guides
### ✨ Code Style
Follow the [.NET Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions). Key points include:
- Prefer automatic properties over public fields.
- Use `var` for local variables when the type is obvious.
- Avoid magic strings and numbers. Use constants or enums.

### 📒 C# Specific Notes
- Place `{` on the same line as the preceding code.
- Use PascalCase for class names and method names, and camelCase for local variables.
- Follow the [Microsoft Naming Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines).

## 👨‍👩‍👧‍👦 Community Support
For any questions, feel free to open a GitHub Discussion or reach out via issues. We encourage everyone to share knowledge and lend a hand to fellow collaborators.

Thank you for contributing to NeoBleeper and helping build something incredible!
