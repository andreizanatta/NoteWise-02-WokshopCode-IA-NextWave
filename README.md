# NotesWise API

A .NET 9 Web API that provides data storage endpoints for the NotesWise React application, using Supabase for authentication and in-memory storage for data.

## Phase 1 Implementation Complete ✅

This implementation covers:

- ✅ .NET 9 Web API with minimal APIs
- ✅ Supabase JWT authentication middleware  
- ✅ In-memory storage service with singleton scope
- ✅ Complete CRUD endpoints with proper user filtering
- ✅ CORS configuration for React app
- ✅ API testing setup

## Architecture

### Hybrid Authentication Approach
- **Frontend Authentication**: Continues using Supabase Auth (login, register, session management)
- **API Authorization**: Validates Supabase JWT tokens to authorize data operations
- **User Identification**: Extracts `user_id` from validated JWT tokens for data filtering

### Data Models
- **Categories**: User-specific note categories with color coding
- **Notes**: Main content entities with markdown support, summaries, and audio URLs
- **Flashcards**: Question/answer pairs linked to specific notes

### In-Memory Storage
- Thread-safe `ConcurrentDictionary` collections for each entity type
- Singleton service registration ensures data persistence during application lifetime
- Automatic cleanup of related data (flashcards when notes deleted, etc.)

## Project Structure

```
NotesWise.API/
├── Models/                     # Data models and request/response DTOs
│   ├── Category.cs
│   ├── Note.cs
│   └── Flashcard.cs
├── Services/                   # Business logic services
│   ├── IDataStore.cs
│   └── InMemoryDataStore.cs
├── Middleware/                 # Custom middleware
│   └── SupabaseAuthMiddleware.cs
├── Endpoints/                  # API endpoint definitions
│   ├── CategoryEndpoints.cs
│   ├── NoteEndpoints.cs
│   └── FlashcardEndpoints.cs
├── Extensions/                 # Extension methods
│   └── HttpContextExtensions.cs
├── Program.cs                  # Application configuration
└── NotesWise.API.http         # HTTP test requests
```

## API Endpoints

### Categories
- `GET /api/categories` - Get user categories
- `POST /api/categories` - Create category
- `PUT /api/categories/{id}` - Update category  
- `DELETE /api/categories/{id}` - Delete category

### Notes
- `GET /api/notes` - Get user notes (with optional `?categoryId=` filter)
- `GET /api/notes/{id}` - Get specific note
- `POST /api/notes` - Create note
- `PUT /api/notes/{id}` - Update note
- `DELETE /api/notes/{id}` - Delete note

### Flashcards
- `GET /api/flashcards` - Get all user flashcards
- `GET /api/notes/{noteId}/flashcards` - Get flashcards for specific note
- `POST /api/notes/{noteId}/flashcards` - Create flashcards for note
- `DELETE /api/flashcards/{id}` - Delete flashcard

### Health
- `GET /health` - Health check endpoint

## Setup Instructions

### Prerequisites
- .NET 9 SDK
- Supabase JWT secret key

### Configuration

1. **Install dependencies**:
   ```bash
   cd notes-wise-backend/NotesWise/NotesWise.API
   dotnet restore
   ```

2. **Configure Supabase JWT Secret**:
   
   Update `appsettings.json` and `appsettings.Development.json`:
   ```json
   {
     "Supabase": {
       "JwtSecret": "your-supabase-jwt-secret-here"
     }
   }
   ```

   **To get your Supabase JWT secret:**
   - Go to your Supabase project dashboard
   - Navigate to Settings > API
   - Copy the "JWT Secret" value
   - This is used to verify JWT tokens issued by Supabase

3. **Run the API**:
   ```bash
   dotnet run
   ```

   The API will start on `http://localhost:5000` (or `https://localhost:5001`)

### Testing the API

1. **Health Check** (no auth required):
   ```bash
   curl http://localhost:5000/health
   ```

2. **Authenticated Requests**:
   
   First, get a Supabase JWT token from your React app's browser developer tools:
   - Login to the React app
   - Open browser dev tools → Application → Local Storage
   - Find the Supabase session data containing the access_token
   
   Then use it in requests:
   ```bash
   curl -H "Authorization: Bearer YOUR_SUPABASE_JWT_TOKEN" \
        http://localhost:5000/api/categories
   ```

3. **Use the HTTP file**:
   - Open `NotesWise.API.http` in VS Code with REST Client extension
   - Replace `{{supabaseToken}}` with your actual JWT token
   - Click "Send Request" on any endpoint

## Security Features

### JWT Token Validation
- Validates tokens using Supabase's JWT secret
- Verifies token signature, issuer, audience, and expiration
- Extracts user ID from token claims for authorization

### User Data Isolation
- All data operations filtered by authenticated user's ID
- Users can only access their own categories, notes, and flashcards
- Proper error handling for unauthorized access attempts

### CORS Configuration
- Configured for React development servers (ports 3000 and 5173)
- Allows credentials for cookie-based auth flows
- Production-ready CORS configuration included

## Next Steps (Phase 2)

Phase 2 will focus on frontend integration:

1. **Create API Client Service** - Replace Supabase client with HTTP API calls
2. **Update Authentication Flow** - Keep Supabase auth, use tokens for API calls
3. **Replace Data Queries** - Update all components to use new API endpoints
4. **Update Error Handling** - Handle API-specific error responses
5. **Remove Supabase Dependencies** - Keep only auth-related Supabase usage

## Development Notes

### In-Memory Storage Limitations
- Data resets when API restarts
- No persistence between application sessions
- Suitable for development and testing
- Can be easily replaced with database implementation later

### Logging
- Comprehensive logging for authentication and data operations
- Debug-level logging for authentication middleware in development
- Error tracking for troubleshooting

### Error Handling
- Consistent error responses across all endpoints
- Proper HTTP status codes
- User-friendly error messages
- Security-conscious error details (no internal info leaked)

This completes Phase 1 of the migration plan. The API is ready for testing and frontend integration.