# MovieVerse.Frontend

1. Start the existing MovieVerse ASP.NET Core backend on:
   `http://localhost:5033`

2. Serve this `MovieVerse.Frontend` folder on port `5500`.
   From inside the folder, one simple option is:
   ```bash
   python -m http.server 5500
   ```
   Then open `http://localhost:5500`.

3. The API base URL is configured in:
   `js/config.js`

4. Authentication uses the backend JWT bearer token. The frontend stores the JWT in `localStorage` and sends it as `Authorization: Bearer <token>` on protected API requests.

5. CDN libraries used:
   - Bootstrap 5.3.3
   - Font Awesome 6.5.2
   - jQuery 3.7.1
   - jQuery Validation 1.19.5

The frontend uses the browser Fetch API for backend requests and does not require a Node/build step.
