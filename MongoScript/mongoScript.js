// phase2.js
// Full Phase 2 Script - Aligned with Phase 1 Report & Phase 2 Requirements

import mongoose from "mongoose";

// ---------------- CONNECT TO MONGODB ----------------
// Connection string from your Project Requirements
const MONGO_URI =
  "mongodb+srv://Agent:CyO41ftEO2jYc3Jf@agents.jfuv468.mongodb.net/CourseShares";

async function start() {
  try {
    await mongoose.connect(MONGO_URI);
    console.log("MongoDB Connected Successfully");
    
    const db = mongoose.connection.db;
    await run(db);
  } catch (err) {
    console.error("Connection error:", err);
  } finally {
    await mongoose.connection.close();
    console.log("Connection closed.");
  }
}

// ------------------- SCHEMAS & VALIDATION -------------------------
// (Requirement 2.5: Schema Validation)
// Aligned with Phase 1 "Entities and Relationships" (Page 4)

// 1. Address Schema (Embedded in User)
const addressSchema = new mongoose.Schema({
  city: { type: String, required: true },
  country: { type: String, required: true },
  postalCode: String,
});

// 2. Search History Schema (Embedded in User)
// Phase 1: "Keeps track of user search activity"
const searchHistorySchema = new mongoose.Schema({
  keyword: { type: String, required: true },
  timestamp: { type: Date, default: Date.now },
});

// 3. User Schema
// Phase 1: "Name, email, password hash, role (instructor/student), date joined"
const userSchema = new mongoose.Schema({
  name: { type: String, required: [true, "Name is required"] },
  email: { 
    type: String, 
    unique: true, 
    required: [true, "Email is required"],
    match: [/.+\@.+\..+/, "Please fill a valid email address"] // Basic email regex
  },
  passwordHash: { type: String, required: true },
  role: { 
    type: String, 
    enum: ["student", "instructor", "admin"], // Enforce Phase 1 Roles
    required: true 
  },
  dateJoined: { type: Date, default: Date.now },
  address: addressSchema,
  searchHistory: [searchHistorySchema],
});

// 4. Section Schema
// Phase 1: "Major (e.g. Engineering). Contains multiple courses."
const sectionSchema = new mongoose.Schema({
  sectionName: { type: String, required: true, unique: true },
  description: String,
});

// 5. Course Schema
// Phase 1: "Course code, title, description, parent section."
const courseSchema = new mongoose.Schema({
  code: { type: String, required: true, unique: true },
  title: { type: String, required: true },
  description: String,
  sectionId: { 
    type: mongoose.Schema.Types.ObjectId, 
    ref: "Section",
    required: true 
  },
});

// 6. Comment Schema (Embedded in Material)
// Phase 1: "Commenter ID, text, timestamp"
const commentSchema = new mongoose.Schema({
  userId: { type: mongoose.Schema.Types.ObjectId, ref: "User", required: true },
  text: { type: String, required: true },
  timestamp: { type: Date, default: Date.now },
});

// 7. Material Schema
// Phase 1: "Title, file type, upload date, status, view count."
const materialSchema = new mongoose.Schema({
  title: { type: String, required: true },
  type: { type: String, enum: ["file", "link"], required: true }, // File vs Link
  fileLink: { type: String, required: true }, // The URL/Path
  uploadDate: { type: Date, default: Date.now },
  
  // Phase 1 Workload #4: "Approve Resource" -> Needs strict Status
  status: { 
    type: String, 
    enum: ["Pending", "Approved"], 
    default: "Pending" 
  },
  
  // Phase 1 Workload #9: "Display Trending" -> Needs View Count
  viewsCount: { type: Number, default: 0 },
  
  uploadedBy: { 
    type: mongoose.Schema.Types.ObjectId, 
    ref: "User", 
    required: true 
  },
  courseId: { 
    type: mongoose.Schema.Types.ObjectId, 
    ref: "Course", 
    required: true 
  },
  comments: [commentSchema],
});

// ------------------- MODELS -------------------------
const User = mongoose.model("User", userSchema);
const Section = mongoose.model("Section", sectionSchema);
const Course = mongoose.model("Course", courseSchema);
const Material = mongoose.model("Material", materialSchema);

// =====================================================
// ====================== RUN ==========================
// =====================================================

async function run(db) {
  console.log("\n1. Clearing old data...");
  await User.deleteMany({});
  await Section.deleteMany({});
  await Course.deleteMany({});
  await Material.deleteMany({});
  console.log("   Old data cleared.");

  // ---------------- INSERT SAMPLE DATA (Requirement 2.2) ----------------
  console.log("\n2. Inserting 10+ Sample Documents...");

  // --- Users ---
  const users = await User.insertMany([
    {
      name: "Abdelrahman M. Zayed",
      email: "abdelrahman@uni.edu",
      passwordHash: "securehash1",
      role: "student",
      dateJoined: new Date("2023-09-01"),
      address: { city: "Cairo", country: "Egypt", postalCode: "11511" }
    },
    {
      name: "Ahmad Elsayed",
      email: "ahmad@uni.edu",
      passwordHash: "securehash2",
      role: "instructor",
      dateJoined: new Date("2023-08-15"),
      address: { city: "Alexandria", country: "Egypt", postalCode: "21545" }
    },
    {
      name: "System Admin",
      email: "admin@uni.edu",
      passwordHash: "adminpass",
      role: "admin",
      dateJoined: new Date("2023-01-01"),
      address: { city: "Giza", country: "Egypt", postalCode: "12345" }
    }
  ]);
  const [student1, instructor1, admin1] = users;

  // --- Sections ---
  const sections = await Section.insertMany([
    { sectionName: "Computer Science", description: "CS Major and AI" },
    { sectionName: "Civil Engineering", description: "Construction and Infrastructure" }
  ]);
  const [csSection, civSection] = sections;

  // --- Courses ---
  const courses = await Course.insertMany([
    { code: "CSE349", title: "Advanced Databases", description: "MongoDB & NoSQL", sectionId: csSection._id },
    { code: "CSE101", title: "Intro to CS", description: "Programming basics", sectionId: csSection._id },
    { code: "CIV202", title: "Structural Analysis", description: "Forces and Beams", sectionId: civSection._id }
  ]);
  const [cse349, cse101, civ202] = courses;

  // --- Materials ---
  const materials = await Material.insertMany([
    // Approved Materials (High Views for Trending)
    {
      title: "Lecture 1: Intro to NoSQL",
      type: "file",
      fileLink: "http://docs.uni.edu/lec1.pdf",
      uploadedBy: instructor1._id,
      courseId: cse349._id,
      status: "Approved",
      viewsCount: 250,
      uploadDate: new Date("2023-11-01"),
      comments: [{ userId: student1._id, text: "Great intro!", timestamp: new Date() }]
    },
    {
      title: "Beam Loads Chart",
      type: "file",
      fileLink: "http://docs.uni.edu/beams.jpg",
      uploadedBy: admin1._id,
      courseId: civ202._id,
      status: "Approved",
      viewsCount: 180,
      uploadDate: new Date("2023-11-10")
    },
    {
      title: "Python Cheat Sheet",
      type: "link",
      fileLink: "http://cheatsheet.com/python",
      uploadedBy: student1._id,
      courseId: cse101._id,
      status: "Approved",
      viewsCount: 95,
      uploadDate: new Date("2023-11-12")
    },
    // Pending Materials (For "Pending Approval Queue" Report)
    {
      title: "Student Project Draft",
      type: "file",
      fileLink: "http://drive.google.com/draft",
      uploadedBy: student1._id,
      courseId: cse349._id,
      status: "Pending",
      viewsCount: 2,
      uploadDate: new Date("2023-12-01")
    },
    {
      title: "Structural Notes (Unverified)",
      type: "file",
      fileLink: "http://docs.uni.edu/notes_v1.pdf",
      uploadedBy: student1._id,
      courseId: civ202._id,
      status: "Pending",
      viewsCount: 0,
      uploadDate: new Date("2023-12-02")
    }
  ]);

  console.log("   ✓ Inserted 13 Sample Documents.");

  // =====================================================
  // ====================== INDEXES ======================
  // =====================================================
  // (Requirement 2.5: Indexes)
  // Aligned with Phase 1 "Workload Table" (Page 5)

  console.log("\n3. Creating Indexes...");

  // 1. Support Login (Workload #1)
  await User.collection.createIndex({ email: 1 }, { unique: true });

  // 2. Support Search (Workload #6 "Search for Resources")
  // Text index allows searching by keywords in Title
  await Material.collection.createIndex({ title: "text" });

  // 3. Support Trending Display (Workload #9)
  // Sorting by viewsCount descending is frequent
  await Material.collection.createIndex({ viewsCount: -1 });

  // 4. Support Admin Approval (Workload #4)
  // Filtering by "Pending" status
  await Material.collection.createIndex({ status: 1 });

  // 5. Support "View My Uploads" (Workload #8)
  await Material.collection.createIndex({ uploadedBy: 1 });

  // 6. Support Relationships (Phase 1 Relationships)
  await Material.collection.createIndex({ courseId: 1 });
  await Course.collection.createIndex({ sectionId: 1 });

  console.log("   ✓ Indexes created to match Phase 1 Workload.");

  // =====================================================
  // ================== AGGREGATIONS =====================
  // =====================================================
  // (Requirement 2.4: 4 Aggregation Pipelines)

  console.log("\n4. Running Aggregation Reports...");

  // REPORT 1: Trending Materials
  // (Matches Workload #9)
  const trendingReport = await Material.aggregate([
    { $sort: { viewsCount: -1 } },
    { $limit: 3 },
    {
      $lookup: {
        from: "courses",
        localField: "courseId",
        foreignField: "_id",
        as: "courseInfo"
      }
    },
    { $unwind: "$courseInfo" },
    { 
      $project: { 
        _id: 0,
        Title: "$title", 
        Views: "$viewsCount", 
        Course: "$courseInfo.title",
        Status: "$status"
      } 
    }
  ]);
  console.log("\n[Report 1] Trending Materials (Top 3):");
  console.table(trendingReport);

  // REPORT 2: Pending Approval Queue
  // (Matches Workload #4)
  const pendingQueue = await Material.aggregate([
    { $match: { status: "Pending" } },
    {
      $lookup: {
        from: "users",
        localField: "uploadedBy",
        foreignField: "_id",
        as: "uploader"
      }
    },
    { $unwind: "$uploader" },
    { 
      $project: { 
        _id: 0,
        MaterialTitle: "$title", 
        DateUploaded: "$uploadDate", 
        Student: "$uploader.name" 
      } 
    }
  ]);
  console.log("\n[Report 2] Pending Approval Queue:");
  console.table(pendingQueue);

  // REPORT 3: Top Contributors (Gamification)
  const topContributors = await Material.aggregate([
    { $group: { _id: "$uploadedBy", UploadCount: { $sum: 1 } } },
    { $sort: { UploadCount: -1 } },
    {
      $lookup: {
        from: "users",
        localField: "_id",
        foreignField: "_id",
        as: "user"
      }
    },
    { $unwind: "$user" },
    { $project: { _id: 0, Name: "$user.name", Uploads: "$UploadCount" } }
  ]);
  console.log("\n[Report 3] Top Contributors:");
  console.table(topContributors);

  // REPORT 4: Section Activity
  // (Matches Phase 1 Entity Structure)
  const sectionActivity = await Course.aggregate([
    { $group: { _id: "$sectionId", CoursesCount: { $sum: 1 } } },
    {
      $lookup: {
        from: "sections",
        localField: "_id",
        foreignField: "_id",
        as: "section"
      }
    },
    { $unwind: "$section" },
    { $project: { _id: 0, Section: "$section.sectionName", Courses: "$CoursesCount" } }
  ]);
  console.log("\n[Report 4] Section Activity:");
  console.table(sectionActivity);

  console.log("\n=== SCRIPT COMPLETE ===");
}

// Start the script
start();