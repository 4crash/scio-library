# Tailwind CSS and PostCSS Installation for Blazor Project

This document outlines the steps taken to install and configure Tailwind CSS and PostCSS for the Blazor WebAssembly project.

## Prerequisites
- Node.js installed (version 22.20.0 was used)
- npm (comes with Node.js)

## Installation Steps

1. **Initialize npm package**:
   ```
   npm init -y
   ```
   This creates a `package.json` file in the project root.

2. **Install Tailwind CSS, PostCSS, and Autoprefixer**:
   ```
   npm install -D tailwindcss@^3.4.0 postcss autoprefixer
   ```
   Note: Tailwind CSS v4 was initially installed but downgraded to v3 for compatibility with standard setup.

3. **Initialize Tailwind and PostCSS configuration**:
   ```
   npx tailwindcss init -p
   ```
   This creates `tailwind.config.js` and `postcss.config.js` files.

4. **Configure Tailwind content paths**:
   Updated `tailwind.config.js` to include Blazor-specific file extensions:
   ```javascript
   content: ["./Scio/**/*.{razor,html,cshtml}", "./Scio.API/**/*.{razor,html,cshtml}"]
   ```

5. **Add Tailwind directives to CSS**:
   Added the following to the top of `Scio/wwwroot/css/app.css`:
   ```css
   @tailwind base;
   @tailwind components;
   @tailwind utilities;
   ```

6. **Add build scripts to package.json**:
   Added scripts for building and watching CSS:
   ```json
   "build-css": "tailwindcss -i ./Scio/wwwroot/css/app.css -o ./Scio/wwwroot/css/app.min.css",
   "watch-css": "tailwindcss -i ./Scio/wwwroot/css/app.css -o ./Scio/wwwroot/css/app.min.css --watch"
   ```

7. **Build the CSS**:
   ```
   npm run build-css
   ```
   This generates `Scio/wwwroot/css/app.min.css` with Tailwind styles included.

8. **Update index.html**:
   Changed the CSS link in `Scio/wwwroot/index.html` from `css/app.css` to `css/app.min.css`.

## Usage

- To build CSS for production: `npm run build-css`
- To watch for changes during development: `npm run watch-css`
- Tailwind classes can now be used in `.razor` files within the specified content paths.

## Notes
- The original `app.css` still contains the directives and existing styles.
- Bootstrap is still included separately in `index.html`.
- For automatic building during .NET build, consider adding an MSBuild target to run `npm run build-css`.</content>
<parameter name="filePath">c:\Users\tomas\OneDrive\Dokumenty\projs\blazor2\blazor-merged\TAILWINDINST.md