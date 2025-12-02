Install-Package MongoDB.Driver
Install-Package MongoDB.Bson# Course Shares App

WinForms application using MongoDB.Driver for an Education - Courses Shares system.

## Connection

Connection string used in `Program.cs`:

```
mongodb+srv://Agent:CyO41ftEO2jYc3Jf@agents.jfuv468.mongodb.net/
```

Database name used: `CourseShares` (adjust if different).

## Aggregation Pipelines (Mongo Shell)

1. Trending Materials Report

```
db.materials.aggregate([
  { $sort: { views_count: -1 } },
  { $limit: 5 },
  { $lookup: { from: "courses", localField: "course_id", foreignField: "_id", as: "course" } },
  { $addFields: { course_title: { $arrayElemAt: ["$course.title", 0] } } },
  { $project: { _id: 1, title: 1, views_count: 1, course_title: 1 } }
])
```

2. Top Contributors

```
db.materials.aggregate([
  { $group: { _id: "$user_id", uploads: { $sum: 1 } } },
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
  { $project: { _id: 1, title: 1, upload_date: 1, user_id: 1 } }
])
```

4. Section Activity

```
db.courses.aggregate([
  { $group: { _id: "$section_id", courses_count: { $sum: 1 } } },
  { $lookup: { from: "sections", localField: "_id", foreignField: "_id", as: "section" } },
  { $addFields: { section_name: { $arrayElemAt: ["$section.section_name", 0] } } },
  { $project: { section_id: "$_id", courses_count: 1, section_name: 1, _id: 0 } },
  { $sort: { courses_count: -1 } }
])
```

## Run

Create a WinForms project targeting .NET Framework 4.8 or .NET 6 (Windows) and include the `src` folder files. Install MongoDB.Driver via NuGet:

```
Install-Package MongoDB.Driver
```

Start the app; the Dashboard provides buttons to run the four reports and forms for insert/update/delete operations.
