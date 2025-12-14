// phase2.js
// Full Phase 2 Script – CourseShares

import mongoose from "mongoose";

// ================== CONNECTION ==================
const MONGO_URI =
  "mongodb+srv://Agent:nDxPoMdFNw4eadaG@agents.jfuv468.mongodb.net/CourseShares";

async function start() {
  try {
    await mongoose.connect(MONGO_URI);
    console.log("✓ MongoDB Connected");

    const db = mongoose.connection.db;
    await run(db);
  } catch (err) {
    console.error("Connection error:", err);
  } finally {
    await mongoose.connection.close();
    console.log("✓ Connection closed");
  }
}

// ================== SCHEMAS ==================

// Embedded Address
const addressSchema = new mongoose.Schema({
  city: { type: String, required: true },
  country: { type: String, required: true },
  postalCode: String,
});

// Embedded Search History
const searchHistorySchema = new mongoose.Schema({
  keyword: { type: String, required: true },
  timestamp: { type: Date, default: Date.now },
});

// User
const userSchema = new mongoose.Schema({
  name: { type: String, required: true },
  email: {
    type: String,
    unique: true,
    required: true,
    match: [/.+\@.+\..+/, "Invalid email"],
  },
  passwordHash: { type: String, required: true },
  role: {
    type: String,
    enum: ["student", "instructor", "admin"],
    required: true,
  },
  dateJoined: { type: Date, default: Date.now },
  address: addressSchema,
  searchHistory: [searchHistorySchema],
});

// Section
const sectionSchema = new mongoose.Schema({
  sectionName: { type: String, required: true, unique: true },
  description: String,
});

// Course
const courseSchema = new mongoose.Schema({
  code: { type: String, required: true, unique: true },
  title: { type: String, required: true },
  description: String,
  sectionId: {
    type: mongoose.Schema.Types.ObjectId,
    ref: "Section",
    required: true,
  },
});

// Embedded Comment
const commentSchema = new mongoose.Schema({
  userId: { type: mongoose.Schema.Types.ObjectId, ref: "User", required: true },
  text: { type: String, required: true },
  timestamp: { type: Date, default: Date.now },
});

// Material
const materialSchema = new mongoose.Schema({
  title: { type: String, required: true },
  type: { type: String, enum: ["file", "link"], required: true },
  fileLink: { type: String, required: true },
  uploadDate: { type: Date, default: Date.now },
  status: {
    type: String,
    enum: ["Pending", "Approved"],
    default: "Pending",
  },
  viewsCount: { type: Number, default: 0 },
  uploadedBy: {
    type: mongoose.Schema.Types.ObjectId,
    ref: "User",
    required: true,
  },
  courseId: {
    type: mongoose.Schema.Types.ObjectId,
    ref: "Course",
    required: true,
  },
  comments: [commentSchema],
});

// ================== MODELS ==================
const User = mongoose.model("User", userSchema);
const Section = mongoose.model("Section", sectionSchema);
const Course = mongoose.model("Course", courseSchema);
const Material = mongoose.model("Material", materialSchema);

// ================== MAIN RUN ==================
async function run(db) {
  console.log("\n1) Clearing old data...");
  await Promise.all([
    User.deleteMany({}),
    Section.deleteMany({}),
    Course.deleteMany({}),
    Material.deleteMany({}),
  ]);
  console.log("✓ Old data cleared");
// ================== DATABASE-LEVEL VALIDATION ==================
// (MongoDB JSON Schema Validators for all collections)

console.log("\n2) Applying MongoDB Validators...");

// Users
await db.command({
  collMod: "users",
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["name", "email", "passwordHash", "role", "address"],
      properties: {
        name: { bsonType: "string", minLength: 3 },
        email: { 
          bsonType: "string", 
          pattern: "^.+@.+\\..+$" 
        },
        passwordHash: { bsonType: "string", minLength: 6 },
        role: { enum: ["student", "instructor", "admin"] },
        dateJoined: { bsonType: "date" },
        address: {
          bsonType: "object",
          required: ["city", "country"],
          properties: {
            city: { bsonType: "string" },
            country: { bsonType: "string" },
            postalCode: { bsonType: "string" }
          }
        },
        searchHistory: {
          bsonType: "array",
          items: {
            bsonType: "object",
            required: ["keyword", "timestamp"],
            properties: {
              keyword: { bsonType: "string" },
              timestamp: { bsonType: "date" }
            }
          }
        }
      }
    }
  }
});
console.log("✓ Users validator applied");

// Sections
await db.command({
  collMod: "sections",
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["sectionName"],
      properties: {
        sectionName: { bsonType: "string", minLength: 2 },
        description: { bsonType: "string" }
      }
    }
  }
});
console.log("✓ Sections validator applied");

// Courses
await db.command({
  collMod: "courses",
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["code", "title", "sectionId"],
      properties: {
        code: { bsonType: "string", minLength: 4 },
        title: { bsonType: "string", minLength: 3 },
        description: { bsonType: "string" },
        sectionId: { bsonType: "objectId" }
      }
    }
  }
});
console.log("✓ Courses validator applied");

// Materials (extended version)
await db.command({
  collMod: "materials",
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["title", "uploadedBy", "courseId", "uploadDate", "status", "type", "fileLink"],
      properties: {
        title: { bsonType: "string", minLength: 3 },
        uploadedBy: { bsonType: "objectId" },
        courseId: { bsonType: "objectId" },
        uploadDate: { bsonType: "date" },
        status: { enum: ["Pending", "Approved"] },
        type: { enum: ["file", "link"] },
        fileLink: { bsonType: "string" },
        viewsCount: { bsonType: "int", minimum: 0 },
        comments: {
          bsonType: "array",
          items: {
            bsonType: "object",
            required: ["userId", "text"],
            properties: {
              userId: { bsonType: "objectId" },
              text: { bsonType: "string" },
              timestamp: { bsonType: "date" }
            }
          }
        }
      }
    }
  }
});
console.log("✓ Materials validator applied");

// ✅ Validators applied for all main collections

  // ================== USERS ==================
  const users = await User.insertMany([
    {
      name: "Abdelrahman Zayed",
      email: "zayed@courseshares.edu",
      passwordHash: "zayed1234",
      role: "student",
      address: { city: "Cairo", country: "Egypt" },
      searchHistory: [{ keyword: "MongoDB aggregation" }],
    },
    {
      name: "Jana Wael",
      email: "Jana@courseshares.edu",
      passwordHash: "jana1234",
      role: "student",
      address: { city: "Giza", country: "Egypt" },
      searchHistory: [{ keyword: "structural analysis notes" }],
    },
    {
      name: "Ahmad Elsayed",
      email: "ahmad@courseshares.edu",
      passwordHash: "ahmad1234",
      role: "student",
      address: { city: "Alexandria", country: "Egypt" },
    },
    {
      name: "Mariam Shemies",
      email: "mariam@courseshares.edu",
      passwordHash: "mariam1234",
      role: "student",
      address: { city: "New Cairo", country: "Egypt" },
    },
    {
      name: "Hassan Hazem",
      email: "hassan@courseshares.edu",
      passwordHash: "hassan1234",
      role: "student",
      address: { city: "Cairo", country: "Egypt" },
    },
    {
      name: "CourseShares Admin",
      email: "admin@courseshares.edu",
      passwordHash: "adminpass",
      role: "admin",
      address: { city: "Cairo", country: "Egypt" },
    },
    {
      name: "CourseShares Admin 2",
      email: "admin2@courseshares.edu",
      passwordHash: "admin2pass",
      role: "admin",
      address: { city: "Cairo", country: "Egypt" },
    },
  ]);

  const [student1,student2,student3,student4,student5,admin1,admin2] = users;


  // ================== SECTIONS ==================
  const sections = await Section.insertMany([
    {
      sectionName: "Computer Science",
      description: "Databases, AI, Software Engineering",
    },
    {
      sectionName: "Civil Engineering",
      description: "Structures and Construction",
    },
  ]);

  const [csSection, civSection] = sections;

  // ================== COURSES ==================
  const courses = await Course.insertMany([
    {
      code: "CSE349",
      title: "Advanced Database Systems",
      description: "MongoDB, indexing, aggregation",
      sectionId: csSection._id,
    },
    {
      code: "CSE101",
      title: "Introduction to Computer Science",
      description: "Programming fundamentals",
      sectionId: csSection._id,
    },
    {
      code: "CIV202",
      title: "Structural Analysis",
      description: "Beams and frames",
      sectionId: civSection._id,
    },
  ]);

  const [cse349, cse101, civ202] = courses;

  // ================== MATERIALS ==================
  const materials = await Material.insertMany([
    {
      title: "Lecture 01 – Introduction to NoSQL",
      type: "file",
      fileLink:"https://courseshares.edu/materials/cse349/lecture01-nosql.pdf",
      uploadedBy: student1._id,
      courseId: cse349._id,
      status: "Approved",
      viewsCount: 420,
      comments: [
        { userId: student1._id, text: "Very helpful lecture!" },
        { userId: student2._id, text: "Clear explanation of NoSQL models." },
      ],
    },
    {
      title: "MongoDB Indexing Best Practices",
      type: "link",
      fileLink:"https://www.mongodb.com/docs/manual/indexes/",
      uploadedBy: student2._id,
      courseId: cse349._id,
      status: "Approved",
      viewsCount: 310,
    },
    {
      title: "Python Programming Cheat Sheet",
      type: "file",
      fileLink:"https://courseshares.edu/materials/cse101/python-cheatsheet.pdf",
      uploadedBy: student4._id,
      courseId: cse101._id,
      status: "Approved",
      viewsCount: 190,
    },
    {
      title: "Structural Load Calculations – Examples",
      type: "file",
      fileLink:"https://courseshares.edu/materials/civ202/load-calculations.pdf",
      uploadedBy: student5._id,
      courseId: civ202._id,
      status: "Approved",
      viewsCount: 230,
    },
    {
      title: "Database Project Proposal (Draft)",
      type: "file",
      fileLink:"https://courseshares.edu/uploads/drafts/db-project-proposal.pdf",
      uploadedBy: student2._id,
      courseId: cse349._id,
      status: "Pending",
      viewsCount: 6,
    },
    {
      title: "Structural Analysis Summary Notes (Student)",
      type: "file",
      fileLink:"https://courseshares.edu/uploads/drafts/structural-summary.pdf",
      uploadedBy: student1._id,
      courseId: civ202._id,
      status: "Pending",
      viewsCount: 2,
    },
  ]);

  console.log(
    `✓ Inserted ${
      users.length + sections.length + courses.length + materials.length
    } documents`
  );

  // ================== INDEXES ==================
  console.log("\n3) Creating indexes...");

  await User.collection.createIndex({ email: 1 }, { unique: true });
  await User.collection.createIndex({ role: 1 });
  await Material.collection.createIndex({ title: "text" });
  await Material.collection.createIndex({ viewsCount: -1 });
  await Material.collection.createIndex({ status: 1 });
  await Material.collection.createIndex({ uploadedBy: 1 });
  await Material.collection.createIndex({ courseId: 1 });
  await Course.collection.createIndex({ sectionId: 1 });
  await Material.collection.createIndex({ status: 1, uploadDate: -1 });
  await Material.collection.createIndex({ courseId: 1, status: 1 });
  await Material.collection.createIndex({ uploadedBy: 1, viewsCount: -1 });
  console.log("✓ Indexes created");

// ================== CRUD: CREATE ==================
console.log("\n4) CRUD – CREATE");

const newMaterial = await Material.create({
  title: "MongoDB Transactions Explained",
  type: "link",
  fileLink: "https://www.mongodb.com/docs/manual/core/transactions/",
  uploadedBy: student3._id,
  courseId: cse349._id,
  status: "Pending",
  viewsCount: 0,
});
console.log("✓ New material created:", newMaterial.title);
// ================== CRUD: READ ==================
console.log("\n5) CRUD – READ");

const approvedMaterials = await Material.find(
  { status: "Approved" },
  { title: 1, viewsCount: 1, _id: 0 }
);

console.table(approvedMaterials);
// ================== CRUD: UPDATE ==================
console.log("\n6) CRUD – UPDATE");

// Approve a pending material
const approvedMaterial = await Material.findByIdAndUpdate(
  newMaterial._id,
  { $set: { status: "Approved" } },
  { new: true }
);

console.log("✓ Material approved:", approvedMaterial.title);

// Increment views count (simulate viewing)
await Material.updateOne(
  { _id: approvedMaterial._id },
  { $inc: { viewsCount: 1 } }
);

console.log("✓ Views incremented");
// ================== CRUD: DELETE ==================
console.log("\n7) CRUD – DELETE");

const deleted = await Material.findOneAndDelete({
  title: "Structural Analysis Summary Notes (Student)",
});

if (deleted) {
  console.log("✓ Deleted material:", deleted.title);
} else {
  console.log("No material found to delete");
}

  // ================== AGGREGATIONS ==================
  console.log("\n8) Aggregation Reports");

  // Trending Materials
  const trending = await Material.aggregate([
    { $sort: { viewsCount: -1 } },
    { $limit: 3 },
    {
      $lookup: {
        from: "courses",
        localField: "courseId",
        foreignField: "_id",
        as: "course",
      },
    },
    { $unwind: "$course" },
    {
      $project: {
        _id: 0,
        Title: "$title",
        Views: "$viewsCount",
        Course: "$course.title",
      },
    },
  ]);
  console.table(trending);

  // Pending Approval Queue
  const pending = await Material.aggregate([
    { $match: { status: "Pending" } },
    {
      $lookup: {
        from: "users",
        localField: "uploadedBy",
        foreignField: "_id",
        as: "user",
      },
    },
    { $unwind: "$user" },
    {
      $project: {
        _id: 0,
        Material: "$title",
        UploadedBy: "$user.name",
        Date: "$uploadDate",
      },
    },
  ]);
  console.table(pending);

  // Top Contributors
  const contributors = await Material.aggregate([
    { $group: { _id: "$uploadedBy", uploads: { $sum: 1 } } },
    { $sort: { uploads: -1 } },
    {
      $lookup: {
        from: "users",
        localField: "_id",
        foreignField: "_id",
        as: "user",
      },
    },
    { $unwind: "$user" },
    { $project: { _id: 0, Name: "$user.name", Uploads: "$uploads" } },
  ]);
  console.table(contributors);

  // Section Activity
  const sectionActivity = await Course.aggregate([
    { $group: { _id: "$sectionId", courseCount: { $sum: 1 } } },
    {
      $lookup: {
        from: "sections",
        localField: "_id",
        foreignField: "_id",
        as: "section",
      },
    },
    { $unwind: "$section" },
    {
      $project: {
        _id: 0,
        Section: "$section.sectionName",
        Courses: "$courseCount",
      },
    },
  ]);
  console.table(sectionActivity);

  console.log("\n=== PHASE 2 COMPLETE ===");
}

start();
