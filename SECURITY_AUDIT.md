# Security Audit Report - Scio Library Management System

## Critical Issues 🔴

### 1. **No Authentication/Authorization**
- **Severity**: CRITICAL
- **Issue**: No user authentication system. Anyone can access all endpoints
- **Impact**: Unauthorized access to borrow/return books, modify data
- **Fix Required**: 
  - Implement JWT authentication
  - Add user login page
  - Protect API endpoints with `[Authorize]` attribute
  - Add role-based access control (RBAC)

### 2. **User Identity Not Tracked Securely**
- **Severity**: CRITICAL
- **Issue**: User identity comes from untrusted client input (userName string). No user ID tracking.
- **Problem Areas**:
  - BorrowModal accepts any username without verification
  - books.json stores user names as plain text (e.g., "john.doe@example.com")
  - Anyone can claim to be anyone
- **Impact**: User accountability compromised, cannot verify who actually borrowed books
- **Fix Required**:
  - Replace userName with authenticated user ID from JWT token
  - Track borrow records by user ID, not name
  - Implement user table/database

### 3. **Sensitive Data Exposed in JSON File**
- **Severity**: CRITICAL
- **Issue**: User email addresses and borrow history stored unencrypted in books.json
- **Problem Areas**:
  - books.json is plain text, world-readable if accessible
  - Borrow records contain personal identifiable information (PII)
  - File permissions likely overly permissive
- **Impact**: Privacy violation, data breach risk
- **Fix Required**:
  - Move to encrypted database (SQL Server, PostgreSQL)
  - Implement data encryption at rest
  - Add file-level access controls
  - Never store PII in plain text files

### 4. **CORS Policy Too Permissive**
- **Severity**: HIGH
- **Issue**: Multiple localhost origins allowed with AllowCredentials
```csharp
policy.WithOrigins(
    "http://localhost:5186",
    "http://localhost:5173",
    "http://localhost:3000",
    "https://localhost:7207",
    "https://localhost:7095"
).AllowAnyMethod()
 .AllowAnyHeader()
 .AllowCredentials();
```
- **Impact**: Potential CSRF attacks, credential theft
- **Fix Required**:
  - Remove AllowAnyMethod() and AllowAnyHeader() - be specific
  - Don't use AllowCredentials with multiple origins in production
  - Implement strict CORS policy

---

## High Priority Issues 🟠

### 5. **No HTTPS Enforcement in Production**
- **Severity**: HIGH
- **Issue**: UseHttpsRedirection() only works in production, not enforced in development
- **Impact**: Man-in-the-middle attacks, credential interception
- **Fix Required**:
  - Force HTTPS in all environments
  - Add HSTS (HTTP Strict Transport Security) headers
  - Add X-Frame-Options header

### 6. **No Input Validation on API Layer**
- **Severity**: HIGH
- **Issue**: While request models exist with validation, they're not enforced
```csharp
// BookController currently doesn't validate ModelState properly
if (!ModelState.IsValid)
    return BadRequest(ModelState);
```
- **Problem Areas**:
  - No global validation filter
  - Validation errors not consistent
  - No sanitization of HTML/script content
- **Impact**: XSS, injection attacks
- **Fix Required**:
  - Create global ValidateModelAttribute filter
  - Sanitize all string inputs (HtmlEncode)
  - Validate all inputs server-side

### 7. **User Input Reflected in UI Without Encoding**
- **Severity**: HIGH
- **Issue**: User names and search terms displayed directly in Blazor components
```razor
<BorrowModal BookAuthor="selectedBookAuthor" ... />
```
- **Impact**: XSS attacks possible if userName contains script tags
- **Fix Required**:
  - Blazor auto-encodes by default, but verify all bindings
  - Use @Html.Encode() for user data
  - Implement Content Security Policy (CSP) headers

### 8. **No Rate Limiting**
- **Severity**: HIGH
- **Issue**: No protection against brute force or DoS attacks
- **Impact**: API abuse, enumeration attacks
- **Fix Required**:
  - Implement AspNetCoreRateLimit NuGet package
  - Add rate limiting middleware
  - Limit requests per IP/user

### 9. **No Logging/Audit Trail**
- **Severity**: HIGH
- **Issue**: No audit log of who did what and when
- **Impact**: Cannot track malicious activity, debug security issues
- **Fix Required**:
  - Implement Serilog or similar logging framework
  - Log all significant operations (add, borrow, return, login)
  - Include timestamp, user ID, action, result

### 10. **No API Versioning**
- **Severity**: MEDIUM
- **Issue**: All endpoints are v0/unversioned
- **Impact**: Breaking changes affect all clients
- **Fix Required**:
  - Add API versioning (e.g., /api/v1/book)
  - Version all endpoints

---

## Medium Priority Issues 🟡

### 11. **Weak Input Validation on UserName**
- **Severity**: MEDIUM
- **Issue**: Only checks length (2-100 chars) and basic regex
```csharp
[RegularExpression(@"^[a-zA-Z\s\-\.']*$", ...)
public string UserName { get; set; }
```
- **Problem**: Email addresses pass validation but might contain special chars
- **Fix Required**:
  - Use built-in Email validation: `[EmailAddress]` if storing emails
  - Or validate as display name only

### 12. **No Request Size Limits**
- **Severity**: MEDIUM
- **Issue**: No MaxRequestBodySize configured
- **Impact**: DoS via massive payloads
- **Fix Required**:
  - Add `services.Configure<FormOptions>(options => options.ValueLengthLimit = 1000);`
  - Set content length limits

### 13. **No CSRF Protection**
- **Severity**: MEDIUM
- **Issue**: No anti-CSRF tokens
- **Impact**: CSRF attacks possible from malicious sites
- **Fix Required**:
  - Add CSRF token validation
  - Use double-submit cookie pattern
  - Implement ValidateAntiForgeryToken

### 14. **Synchronous File Operations**
- **Severity**: LOW
- **Issue**: books.json is synchronous in constructor
```csharp
_books = LoadBooksFromFile().Result; // Blocks on startup
```
- **Impact**: Potential deadlocks, scalability issues
- **Fix Required**:
  - Make async throughout
  - Consider using database instead of JSON file

---

## Low Priority Issues 🟢

### 15. **No Content Security Policy**
- **Severity**: LOW
- **Issue**: No CSP headers configured
- **Fix**: Add `Content-Security-Policy` response headers

### 16. **No X-Content-Type-Options Header**
- **Severity**: LOW
- **Issue**: Missing security header
- **Fix**: Add middleware to set `X-Content-Type-Options: nosniff`

### 17. **Error Messages Too Verbose**
- **Severity**: LOW
- **Issue**: Full exception messages exposed to clients
- **Fix**: Show generic errors to clients, log detailed errors

---

## Implementation Priority

### Phase 1 (CRITICAL - Do First):
1. Add JWT authentication system
2. Replace userName with authenticated user ID
3. Migrate to encrypted database
4. Remove AllowCredentials from CORS

### Phase 2 (HIGH - Do Next):
5. Implement global validation filter
6. Add rate limiting
7. Add logging/audit trail
8. Add security headers (HSTS, CSP, X-Frame-Options)

### Phase 3 (MEDIUM - Nice to Have):
9. Strengthen input validation
10. Add request size limits
11. Add CSRF protection
12. Implement API versioning

---

## Security Headers to Add

```csharp
// In Program.cs
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
    await next();
});
```

---

## Files Most at Risk

- `books.json` - Contains PII unencrypted
- `BookController.cs` - No authorization checks
- `BookService.cs` - Takes untrusted userName input
- `Program.cs` - Overly permissive CORS
- `BorrowModal.razor` - Accepts unverified user input

---

## Next Steps

Would you like me to implement:
1. JWT authentication system?
2. Database migration (SQL)?
3. Global validation filter?
4. Security headers middleware?
5. Rate limiting?
