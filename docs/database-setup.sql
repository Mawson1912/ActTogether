-- ActTogether Database Setup

-- This SQL file sets up the database and populates it with dummy data for demonstration purposes.

---

-- Step 1: Create Database

-- Open SQL Server Management Studio, connect to your local SQL Server instance (localhost or .\SQLEXPRESS), then run:


-- Create the database (you can use any name, but "ActTogether" is recommended)
CREATE DATABASE [ActTogether];
GO

USE [ActTogether];
GO

---

-- Step 2: Create Tables


-- Users table
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50),
    Email NVARCHAR(100),
    PasswordHash NVARCHAR(200),
    FullName NVARCHAR(100),
    Bio NVARCHAR(500),
    ProfileImageUrl NVARCHAR(500),
    CreatedAt DATETIME NOT NULL,
    IsAdmin BIT NOT NULL DEFAULT 0
);

-- Friendships table
CREATE TABLE Friendships (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    User1Id INT NOT NULL,
    User2Id INT NOT NULL,
    Status NVARCHAR(20),
    RequestedBy INT NOT NULL,
    BlockedBy INT NULL,
    CreatedAt DATETIME NOT NULL
);

-- Posts table
CREATE TABLE Posts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Title NVARCHAR(200),
    VideoUrl NVARCHAR(500),
    Description NVARCHAR(2000),
    PostedAt DATETIME NOT NULL
);

-- Messages table
CREATE TABLE Messages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SenderId INT NOT NULL,
    ReceiverId INT NOT NULL,
    Content NVARCHAR(2000),
    SentAt DATETIME NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0
);

-- ContactMessages table
CREATE TABLE ContactMessages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Message NVARCHAR(2000),
    SentAt DATETIME NOT NULL
);

-- Reports table
CREATE TABLE Reports (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ReporterId INT NOT NULL,
    ReportedUserId INT NOT NULL,
    Reason NVARCHAR(500),
    CreatedAt DATETIME NOT NULL
);

-- Feedbacks table
CREATE TABLE Feedbacks (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PostId INT NOT NULL,
    SenderId INT NOT NULL,
    Content NVARCHAR(2000),
    CreatedAt DATETIME NOT NULL
);

-- DismissedPosts table
CREATE TABLE DismissedPosts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    PostId INT NOT NULL,
    DismissedAt DATETIME NOT NULL
);

---

-- Step 3: Insert Dummy Data

-- Users (5 accounts)

/* 
Login credentials (all passwords: Password123!):**
- alice / Password123!
- bob / Password123!
- charlie / Password123!
- dave / Password123!
- admin / Password123!
*/



-- Regular users
INSERT INTO Users (Username, Email, PasswordHash, FullName, Bio, CreatedAt, IsAdmin)
VALUES
('alice', 'alice@example.com', '$2a$12$WI9R9t3fKjlJGjCgU6CCx.2VytpL7gkH/yRydKElV.9fFxpqBnOoi', 'Alice Smith', 'Professional actor based in Dublin. Specializing in drama and thriller roles. Recently booked a supporting role in a BBC series.', DATEADD(day, -30, GETDATE()), 0),
('bob', 'bob@example.com', '$2a$12$WI9R9t3fKjlJGjCgU6CCx.2VytpL7gkH/yRydKElV.9fFxpqBnOoi', 'Bob Johnson', 'Working actor from Cork. Focus on commercial and TV pilot auditions. Always refining my self-tape setup.', DATEADD(day, -25, GETDATE()), 0),
('charlie', 'charlie@example.com', '$2a$12$WI9R9t3fKjlJGjCgU6CCx.2VytpL7gkH/yRydKElV.9fFxpqBnOoi', 'Charlie Brown', 'Stage and screen actor based in Galway. Trained at The Lir. Currently auditioning for feature film roles.', DATEADD(day, -20, GETDATE()), 0),
('dave', 'dave@example.com', '$2a$12$WI9R9t3fKjlJGjCgU6CCx.2VytpL7gkH/yRydKElV.9fFxpqBnOoi', 'Dave Wilson', 'New to professional acting. Building my self-tape portfolio and learning the industry.', DATEADD(day, -15, GETDATE()), 0),

-- Admin account (no posts, no friendships)
('admin', 'admin@example.com', '$2a$12$WI9R9t3fKjlJGjCgU6CCx.2VytpL7gkH/yRydKElV.9fFxpqBnOoi', 'System Admin', 'Platform administrator.', DATEADD(day, -5, GETDATE()), 1);


-- Friendships (Alice, Bob, Charlie are friends; Dave has no friends)


-- Alice -> Bob (accepted)
INSERT INTO Friendships (User1Id, User2Id, Status, RequestedBy, CreatedAt)
VALUES (1, 2, 'Accepted', 1, DATEADD(day, -20, GETDATE()));

-- Alice -> Charlie (accepted)
INSERT INTO Friendships (User1Id, User2Id, Status, RequestedBy, CreatedAt)
VALUES (1, 3, 'Accepted', 1, DATEADD(day, -18, GETDATE()));

-- Bob -> Charlie (accepted)
INSERT INTO Friendships (User1Id, User2Id, Status, RequestedBy, CreatedAt)
VALUES (2, 3, 'Accepted', 2, DATEADD(day, -15, GETDATE()));


-- Posts (Each regular user posts 2 videos using the 8 Google Drive links - self-tape auditions for specific roles)

-- Alice's posts (videos 1, 2)
INSERT INTO Posts (UserId, Title, VideoUrl, Description, PostedAt)
VALUES
(1, 'Detective Sarah - Crime Drama Series', 'https://drive.google.com/file/d/1U_sC8SUY1syUqKzLfu2rtCOZJAlUoY8h/view?usp=drive_link', 'Auditioning for the lead detective role in a new BBC crime drama. This is the tape that got me the callback! BOOKED THE ROLE.', DATEADD(day, -19, GETDATE())),
(1, 'Elena - Thriller Feature Film', 'https://drive.google.com/file/d/16Ar5eCtIHR3gdyNLEf76EezMpJV9Fv_l/view?usp=drive_link', 'Audition for supporting role in psychological thriller. Looking for feedback on the emotional range in the second scene.', DATEADD(day, -10, GETDATE()));

-- Bob's posts (videos 3, 4)
INSERT INTO Posts (UserId, Title, VideoUrl, Description, PostedAt)
VALUES
(2, 'Tech Startup Founder - Commercial', 'https://drive.google.com/file/d/1ydYOUWSVuViO5Q_MiVJ8x0qTOIRb-x4R/view?usp=drive_link', 'Commercial audition for tech company. Focusing on natural delivery and connecting with camera. BOOKED THE ROLE.', DATEADD(day, -17, GETDATE())),
(2, 'Jack - TV Pilot', 'https://drive.google.com/file/d/1oW7voNy8msJKYq8ZQgASfa6meBwzpuzI/view?usp=drive_link', 'Pilot audition for comedy series. Need feedback on timing - is it too fast or just right?', DATEADD(day, -8, GETDATE()));

-- Charlie's posts (videos 5, 6)
INSERT INTO Posts (UserId, Title, VideoUrl, Description, PostedAt)
VALUES
(3, 'Marcus - Period Drama Feature', 'https://drive.google.com/file/d/1O1DzjtUW2VycncgDV1bda3F9Sozqi7lQ/view?usp=drive_link', 'Audition for 1920s period piece. Working on the accent - does it sound authentic?', DATEADD(day, -14, GETDATE())),
(3, 'Liam - Action Series', 'https://drive.google.com/file/d/1uQqjj_4eCrX1hrS4ZccwkqmYVybCugAo/view?usp=drive_link', 'Action series audition. This tape got me the role! Physical preparation was key. BOOKED THE ROLE.', DATEADD(day, -5, GETDATE()));

-- Dave's posts (videos 7, 8) - but no one will see these since he has no friends
INSERT INTO Posts (UserId, Title, VideoUrl, Description, PostedAt)
VALUES
(4, 'Supporting Role - Indie Film', 'https://drive.google.com/file/d/1idozPBJHCDvuCAqY67N-lQJrmxr7jBjg/view?usp=drive_link', 'My first serious self-tape audition. Building confidence with the camera setup.', DATEADD(day, -12, GETDATE())),
(4, 'Commercial - Brand Ambassador', 'https://drive.google.com/file/d/1ToT2Hecni2gyTthVMi478g3rxoFL6Xo5/view?usp=drive_link', 'Commercial audition. Working on being more natural and less rehearsed.', DATEADD(day, -3, GETDATE()));

-- Messages (Conversations between friends)

-- Alice to Bob
INSERT INTO Messages (SenderId, ReceiverId, Content, SentAt, IsRead)
VALUES (1, 2, 'Hey Bob! Just watched your commercial tape - the natural delivery was perfect. How did it go?', DATEADD(hour, -48, GETDATE()), 1);

-- Bob to Alice
INSERT INTO Messages (SenderId, ReceiverId, Content, SentAt, IsRead)
VALUES (2, 1, 'Thanks Alice! I booked it! They loved the authentic feel. We should compare notes on self-tape setups sometime.', DATEADD(hour, -47, GETDATE()), 1);

-- Charlie to Alice
INSERT INTO Messages (SenderId, ReceiverId, Content, SentAt, IsRead)
VALUES (3, 1, 'Congrats on booking that detective role! Your tape was incredible - what was your prep process?', DATEADD(hour, -24, GETDATE()), 1);

-- Alice to Charlie
INSERT INTO Messages (SenderId, ReceiverId, Content, SentAt, IsRead)
VALUES (1, 3, 'Thanks! I spent a week researching real detectives. The key was making it feel lived-in, not performed.', DATEADD(hour, -23, GETDATE()), 1);


-- ContactMessages (Admin receives messages from users)

-- Alice contacts admin
INSERT INTO ContactMessages (UserId, Message, SentAt)
VALUES (1, 'Hi, I''m having trouble uploading my self-tape video. Is there a file size limit?', DATEADD(day, -2, GETDATE()));

-- Bob contacts admin
INSERT INTO ContactMessages (UserId, Message, SentAt)
VALUES (2, 'Feature request: It would be great to add a section for sharing audition tips and resources.', DATEADD(day, -1, GETDATE()));

-- Charlie contacts admin
INSERT INTO ContactMessages (UserId, Message, SentAt)
VALUES (3, 'Love the platform! It''s really helping me get feedback on my audition tapes.', DATEADD(hour, -6, GETDATE()));

-- Feedbacks (Friends give feedback on each other's posts)

-- Bob gives feedback on Alice's thriller audition
INSERT INTO Feedbacks (PostId, SenderId, Content, CreatedAt)
VALUES (2, 2, 'Great emotional range in the second scene! The vulnerability in the breakdown moment was perfect. Maybe add a slight pause before the final line to let it land.', DATEADD(hour, -40, GETDATE()));

-- Charlie gives feedback on Alice's detective tape (successful audition)
INSERT INTO Feedbacks (PostId, SenderId, Content, CreatedAt)
VALUES (1, 3, 'This is masterful! The way you underplayed the authority while still commanding the scene is exactly what casting directors look for. Taking notes for my next detective role!', DATEADD(hour, -36, GETDATE()));

-- Alice gives feedback on Bob's commercial tape (successful audition)
INSERT INTO Feedbacks (PostId, SenderId, Content, CreatedAt)
VALUES (3, 1, 'The authenticity is spot on! You sound like a real founder, not an actor. That''s what made this tape work - it never felt like a performance.', DATEADD(hour, -35, GETDATE()));

-- Charlie gives feedback on Bob's TV pilot audition
INSERT INTO Feedbacks (PostId, SenderId, Content, CreatedAt)
VALUES (4, 3, 'The timing works for this character - it feels energetic without rushing. Maybe try varying the volume slightly on the punchlines to add more texture.', DATEADD(hour, -20, GETDATE()));

-- Alice gives feedback on Charlie's period drama audition
INSERT INTO Feedbacks (PostId, SenderId, Content, CreatedAt)
VALUES (5, 1, 'The accent sounds solid! The rhythm of the speech feels right for the period. Consider adding more physical stillness - period actors often hold themselves differently.', DATEADD(hour, -18, GETDATE()));

-- Bob gives feedback on Charlie's action tape (successful audition)
INSERT INTO Feedbacks (PostId, SenderId, Content, CreatedAt)
VALUES (6, 2, 'The physical commitment is clear! That''s what sets action tapes apart. The intensity never felt forced - great work on the fight choreography timing.', DATEADD(hour, -12, GETDATE()));

-- Reports (Alice reports Dave)

-- Alice reports Dave
INSERT INTO Reports (ReporterId, ReportedUserId, Reason, CreatedAt)
VALUES (1, 4, 'This user is posting inappropriate content and spamming the platform.', DATEADD(day, -1, GETDATE()));

---

/* Step 4: Configure Application

1. Open `appsettings.json` in the ActTogether project
2. Update the connection string to match your local SQL Server setup:

**For SQL Server (localhost):**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=ActTogether;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**For SQL Server Express:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=ActTogether;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

## Step 5: Run the Application

1. Open the project in Visual Studio or run `dotnet run` in the project directory
2. Log in using the credentials listed above

---

## Test Scenarios

1. **Friend Feed:** Log in as alice, bob, or charlie — you should see self-tape auditions from all three friends
2. **Empty Feed:** Log in as dave — you should see no posts (has no friends)
3. **Feedback on Audition Tapes:** Log in as alice, view Bob's or Charlie's posts, click "Give Feedback" to submit feedback on their audition tapes
4. **Admin Dashboard:** Log in as admin — navigate to Admin Dashboard to view Contact Messages and Reports
5. **User Deletion:** As admin, go to Reports and click "Remove User" to delete Dave (this removes all their data)
6. **Messaging:** Log in as alice, go to Bob's profile, click "Message" to start a conversation about auditions

---

## Notes

- **Profile Pictures:** Place profile images in `wwwroot/uploads/profiles/` and update the ProfileImageUrl column in the Users table if desired
- **BCrypt Passwords:** All dummy accounts use the same password "Password123!" (BCrypt hash: $2a$12$WI9R9t3fKjlJGjCgU6CCx.2VytpL7gkH/yRydKElV.9fFxpqBnOoi)
- **Video Links:** The dummy posts use Google Drive links from `Links-for-videos.md`

*/
