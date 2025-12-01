# Scio API Tests

This project contains comprehensive unit tests for the Scio Library API.

## Overview

The test suite covers:
- **BookService**: Service layer tests for business logic
- **BookController**: Controller layer tests for HTTP endpoints

## Test Structure

### BookServiceTests (18 tests)

Tests for the `BookService` class covering:

#### GetAllBooks Tests
- `GetAllBooksAsync_ShouldReturnAllBooks` - Verifies all books are returned
- `GetAllBooksAsync_ShouldReturnListOfBooks` - Verifies correct return type

#### GetBookById Tests
- `GetBookByIdAsync_WithValidId_ShouldReturnBook` - Retrieves existing book
- `GetBookByIdAsync_WithInvalidId_ShouldReturnNull` - Handles missing book

#### SearchBooks Tests
- `SearchBooksAsync_WithValidSearchTerm_ShouldReturnMatchingBooks` - Search by title/author
- `SearchBooksAsync_WithEmptySearchTerm_ShouldReturnAllBooks` - Empty search returns all
- `SearchBooksAsync_WithNonMatchingTerm_ShouldReturnEmptyList` - No results

#### AddBook Tests
- `AddBookAsync_WithValidBook_ShouldAddBook` - Successfully adds new book
- `AddBookAsync_ShouldGenerateUniqueId` - Each book gets unique ID

#### BorrowBook Tests
- `BorrowBookAsync_WithAvailableBook_ShouldSucceed` - Borrows available copy
- `BorrowBookAsync_WithInvalidBookId_ShouldFail` - Fails for missing book
- `BorrowBookAsync_ShouldCreateBorrowRecord` - Creates borrow history entry

#### ReturnBook Tests
- `ReturnBookAsync_WithBorrowedBook_ShouldSucceed` - Returns borrowed book
- `ReturnBookAsync_WithInvalidBookId_ShouldFail` - Fails for missing book

#### GetBorrowedBooks Tests
- `GetBorrowedBooksAsync_ShouldReturnList` - Returns borrowed books list
- `GetBorrowedBooksAsync_AfterBorrow_ShouldIncludeBook` - Borrowed book appears in list

### BookControllerTests (15 tests)

Tests for the `BookController` class using Moq to mock the service layer:

#### GetAllBooks Tests
- `GetAllBooks_ShouldReturnOkWithBooks` - HTTP 200 response
- `GetAllBooks_ShouldCallServiceOnce` - Service called exactly once

#### GetBookById Tests
- `GetBookById_WithValidId_ShouldReturnOkWithBook` - Returns 200 with book
- `GetBookById_WithInvalidId_ShouldReturnNotFound` - Returns 404

#### SearchBooks Tests
- `SearchBooks_WithValidSearchTerm_ShouldReturnOkWithBooks` - Returns matching books
- `SearchBooks_WithNullRequest_ShouldReturnOkWithAllBooks` - Handles null request

#### AddBook Tests
- `AddBook_WithValidRequest_ShouldReturnCreatedAtAction` - Returns 201 Created
- `AddBook_WithNullRequest_ShouldReturnBadRequest` - Returns 400 Bad Request
- `AddBook_ShouldCallServiceOnce` - Service called once

#### BorrowBook Tests
- `BorrowBook_WithValidRequest_ShouldReturnOk` - Returns 200
- `BorrowBook_WithUnavailableBook_ShouldReturnBadRequest` - Returns 400
- `BorrowBook_WithNullRequest_ShouldReturnBadRequest` - Returns 400

#### ReturnBook Tests
- `ReturnBook_WithValidBookId_ShouldReturnOk` - Returns 200
- `ReturnBook_WithInvalidBookId_ShouldReturnBadRequest` - Returns 400

#### ReturnBorrowRecord Tests
- `ReturnBorrowRecord_WithValidRecordId_ShouldReturnOk` - Returns 200
- `ReturnBorrowRecord_WithInvalidRecordId_ShouldReturnBadRequest` - Returns 400

#### GetBorrowedBooks Tests
- `GetBorrowedBooks_ShouldReturnOkWithBorrowedBooks` - Returns 200 with list

## Running Tests

### Run all tests
```bash
dotnet test Scio.API.Tests
```

### Run specific test class
```bash
dotnet test Scio.API.Tests --filter ClassName=Scio.API.Tests.BookServiceTests
```

### Run with verbose output
```bash
dotnet test Scio.API.Tests -v normal
```

### Run with code coverage
```bash
dotnet test Scio.API.Tests /p:CollectCoverage=true
```

## Test Results

- **Total Tests**: 33
- **Test Framework**: xUnit
- **Mocking Framework**: Moq
- **Status**: All tests passing ✓

## Dependencies

- `xunit` - Testing framework
- `xunit.runner.visualstudio` - Visual Studio test runner
- `Moq` - Mocking framework for unit tests
- `Microsoft.AspNetCore.Mvc.Testing` - ASP.NET Core testing utilities

## Notes

- Service tests use real `BookService` instances to test business logic
- Controller tests use mocked `IBookService` to isolate controller behavior
- Tests follow the AAA pattern (Arrange, Act, Assert)
- All nullable reference warnings are handled appropriately
