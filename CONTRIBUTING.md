# Contributing to Heritage Store Management System

Thank you for your interest in contributing to the Heritage Store Management System! This document provides guidelines and information for contributors.

## 🤝 How to Contribute

### Reporting Issues
- Use the GitHub issue tracker to report bugs or request features
- Provide detailed information about the issue
- Include steps to reproduce the problem
- Attach relevant screenshots or logs

### Suggesting Enhancements
- Use the "Enhancement" label for feature requests
- Describe the proposed feature in detail
- Explain the business value and use case
- Consider the impact on existing functionality

## 🛠️ Development Setup

### Prerequisites
- .NET 9.0 SDK
- Node.js 18+
- Visual Studio Code (recommended)
- Git

### Getting Started
1. Fork the repository
2. Clone your fork locally
3. Set up the development environment:
   ```bash
   # Backend setup
   cd Api
   dotnet restore
   dotnet ef database update
   
   # Frontend setup
   cd ../frontend
   npm install
   ```

### Running the Project
```bash
# Start both backend and frontend
./start_full_system.sh

# Or start individually
./start_backend.sh
./start_frontend.sh
```

## 📝 Code Standards

### C# (.NET)
- Follow Microsoft C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public methods
- Implement proper error handling
- Use async/await for I/O operations

### TypeScript/React
- Use TypeScript for type safety
- Follow React best practices
- Use functional components with hooks
- Implement proper error boundaries
- Use Material-UI components consistently

### Database
- Use Entity Framework Core migrations
- Follow database naming conventions
- Add proper indexes for performance
- Implement audit logging for sensitive operations

## 🧪 Testing

### Backend Testing
- Write unit tests for business logic
- Test API endpoints with Postman
- Verify database operations
- Test authentication and authorization

### Frontend Testing
- Test component rendering
- Verify user interactions
- Test form validation
- Check responsive design

## 📋 Pull Request Process

1. **Create a Feature Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make Your Changes**
   - Write clean, well-documented code
   - Add tests for new functionality
   - Update documentation as needed

3. **Commit Your Changes**
   ```bash
   git commit -m "feat: add new feature description"
   ```

4. **Push to Your Fork**
   ```bash
   git push origin feature/your-feature-name
   ```

5. **Create a Pull Request**
   - Provide a clear description of changes
   - Reference any related issues
   - Request review from maintainers

## 🏗️ Architecture Guidelines

### Backend Architecture
- Follow Clean Architecture principles
- Use dependency injection
- Implement proper separation of concerns
- Use DTOs for data transfer
- Implement proper error handling

### Frontend Architecture
- Use React functional components
- Implement proper state management
- Use TypeScript interfaces
- Follow component composition patterns
- Implement proper error handling

## 🔒 Security Considerations

- Never commit sensitive information (passwords, API keys)
- Use parameterized queries to prevent SQL injection
- Implement proper input validation
- Follow OWASP security guidelines
- Test for security vulnerabilities

## 📚 Documentation

### Code Documentation
- Add XML documentation for public APIs
- Use meaningful comments for complex logic
- Document business rules and requirements
- Keep README files updated

### API Documentation
- Use Swagger/OpenAPI annotations
- Provide example requests and responses
- Document error codes and messages
- Keep API documentation current

## 🐛 Bug Reports

When reporting bugs, please include:
- **Description**: Clear description of the issue
- **Steps to Reproduce**: Detailed steps to reproduce the bug
- **Expected Behavior**: What should happen
- **Actual Behavior**: What actually happens
- **Environment**: OS, browser, version information
- **Screenshots**: If applicable

## 💡 Feature Requests

When requesting features, please include:
- **Use Case**: Why this feature is needed
- **Description**: Detailed description of the feature
- **Acceptance Criteria**: What constitutes completion
- **Mockups**: If applicable
- **Priority**: High/Medium/Low

## 🏷️ Issue Labels

- **bug**: Something isn't working
- **enhancement**: New feature or request
- **documentation**: Improvements to documentation
- **help wanted**: Extra attention is needed
- **good first issue**: Good for newcomers
- **priority: high**: High priority issue
- **priority: medium**: Medium priority issue
- **priority: low**: Low priority issue

## 📞 Getting Help

- **GitHub Issues**: Use the issue tracker for questions
- **Discussions**: Use GitHub Discussions for general questions
- **Email**: Contact the maintainers directly
- **Documentation**: Check existing documentation first

## 🎯 Contribution Areas

### High Priority
- Bug fixes
- Security improvements
- Performance optimizations
- Documentation updates

### Medium Priority
- New features
- UI/UX improvements
- Test coverage improvements
- Code refactoring

### Low Priority
- Nice-to-have features
- Cosmetic improvements
- Additional documentation
- Code style improvements

## 📄 License

By contributing to this project, you agree that your contributions will be licensed under the MIT License.

## 🙏 Recognition

Contributors will be recognized in:
- README.md contributors section
- Release notes
- Project documentation
- GitHub contributors page

Thank you for contributing to the Heritage Store Management System! 🚀
