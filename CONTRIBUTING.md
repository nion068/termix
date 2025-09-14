# Contributing to Termix

First off, thank you for considering contributing to Termix! We welcome any help, whether it's reporting a bug, suggesting a feature, or writing code. This document provides a set of guidelines to make the contribution process easy and effective for everyone involved.

## Code of Conduct

This project and everyone participating in it is governed by our [Code of Conduct](./CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code. Please report unacceptable behavior.

## How Can I Contribute?

### Reporting Bugs

If you find a bug, please make sure it hasn't already been reported by searching the [Issues](https://github.com/amrohan/termix/issues) page.

When filing a new bug report, please include as much detail as possible:

-   **A clear and descriptive title** (e.g., "Crash when pasting into a read-only directory").
-   **Steps to reproduce the issue** step-by-step.
-   **What you expected to happen** versus **what actually happened**.
-   **Your environment**:
    -   Operating System (e.g., Windows 11, Ubuntu 22.04, macOS Sonoma).
    -   Terminal application (e.g., Windows Terminal, iTerm2, Alacritty).
    -   Termix version (`termix --version`).
-   Screenshots or GIFs are incredibly helpful!

### Suggesting Enhancements

Feature requests are a great way to contribute. To suggest an enhancement:

1.  **Search the [Issues](https://github.com/amrohan/termix/issues) page** to see if a similar idea has already been discussed.
2.  If not, create a new issue with a clear title (e.g., "Feature Request: Add tab support for multiple directories").
3.  Provide a detailed description of the proposed functionality and explain the problem it solves or the workflow it improves.

### Submitting Pull Requests

If you'd like to contribute code, we'd love your help! Here is the standard workflow:

1.  **Fork** the repository to your own GitHub account.
2.  **Clone** your fork to your local machine: `git clone https://github.com/YOUR_USERNAME/termix.git`
3.  **Create a new branch** for your changes: `git checkout -b feature/my-awesome-feature` or `fix/bug-that-i-fixed`.
4.  Make your changes to the code.
5.  **Commit** your changes with a descriptive commit message (see our [Git Commit Guidelines](#git-commit-guidelines) below).
6.  **Push** your branch to your fork on GitHub: `git push origin feature/my-awesome-feature`.
7.  Open a **Pull Request** from your fork to the `main` branch of the `amrohan/termix` repository.
8.  Provide a clear description of the changes in your pull request.

## Development Setup

To get started with the code, you'll need:

-   **.NET 9 SDK** or later.
-   A Git client.

Once you've cloned the repository, you can set up your environment:

```bash
# Navigate to the project directory
cd termix

# Restore all .NET dependencies
dotnet restore

# Build the project to ensure everything compiles
dotnet build

# Run Termix locally to test your changes
dotnet run
```

You can now open the project in your favorite editor (like Visual Studio Code, JetBrains Rider, or Visual Studio) and start coding.

## Coding and Style Guidelines

-   **Follow existing code style**: Try to match the formatting and patterns you see in the rest of the codebase. The project generally follows standard Microsoft C# conventions.
-   **Keep it simple**: Prefer clear, readable code over overly complex solutions.
-   **No tests required**: While tests are appreciated if you know how, **they are not required for a pull request to be accepted**. We value the functional contribution above all.
-   **Update documentation**: If your change affects user-facing functionality (like a new keybinding), please mention it in your pull request description so the official documentation can be updated.

### Git Commit Guidelines

We use a conventional commit format to keep the history clean and readable. Please format your commit messages like this:

```
<type>: <subject>

<optional body>
```

**Common types:**

-   `feat`: A new feature.
-   `fix`: A bug fix.
-   `docs`: Changes to documentation.
-   `style`: Formatting changes that don't affect code logic.
-   `refactor`: Code changes that neither fix a bug nor add a feature.
-   `chore`: Build process, package manager, or other maintenance updates.

**Example:**

```
feat: Add scrollable help screen with '?' key
```
---

Thank you again for your interest in making Termix better