# Course Shares App

WinForms app for sharing course materials, built on .NET 6 (Windows) and MongoDB Atlas.

## Features

- Upload materials as files or web links (PDF, Word, PowerPoint, TXT).
- Store file paths or URLs in `materials.fileLink`
- Dashboard shows only Approved materials; Pending hidden for non-admins.
- Reports: Trending, Contributors, Pending Queue, Sections activity.

## Prerequisites

- .NET 6 SDK (Windows).
- MongoDB Atlas cluster with database `CourseShares`.
- NuGet packages: `MongoDB.Driver` and `MongoDB.Bson`.

## Setup

1. Install dependencies:

```
Install-Package MongoDB.Driver
Install-Package MongoDB.Bson
```

2. Configure connection string. Prefer using an environment variable:

```
setx MONGODB_CONNECTION_STRING "mongodb+srv://<username>:<password>@<cluster>.mongodb.net/"
```

The app reads the connection string in `DatabaseContext` and connects to database `CourseShares`.

3. Run:

```
dotnet build
dotnet run
```

## Data Model Notes


- Approved-only views: The dashboard filters `materials.status == "Approved"`.

## Aggregation Pipelines (Mongo Shell)

1. Trending Materials Report

```
db.materials.aggregate([
  { $sort: { views_count: -1 } },
  { $limit: 5 },
  { $lookup: { from: "courses", localField: "courseId", foreignField: "_id", as: "course" } },
  { $addFields: { course_title: { $arrayElemAt: ["$course.title", 0] } } },
  { $project: { _id: 1, title: 1, views_count: 1, course_title: 1 } }
])
```

2. Top Contributors

```
db.materials.aggregate([
  { $group: { _id: "$uploadedBy", uploads: { $sum: 1 } } },
  { $lookup: { from: "users", localField: "_id", foreignField: "_id", as: "user" } },
  { $addFields: { user_name: { $arrayElemAt: ["$user.name", 0] } } },
  { $project: { user_id: "$_id", uploads: 1, user_name: 1, _id: 0 } },
  { $sort: { uploads: -1 } }
])
```

3. Pending Approval Queue

```
db.materials.aggregate([
  { $match: { status: "Pending" } },
  { $project: { _id: 1, title: 1, uploadDate: 1, uploadedBy: 1 } }
])
```

4. Section Activity

```
db.sections.aggregate([
  { $lookup: { from: "courses", localField: "_id", foreignField: "sectionId", as: "courses" } },
  { $project: { Section: { $ifNull: ["$sectionName", "(Unnamed)"] }, TotalCourses: { $size: "$courses" }, _id: 0 } }
])
```


