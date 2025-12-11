using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using CourseSharesApp.Data;

namespace CourseSharesApp.Services
{
    public class ReportService
    {
        private readonly DatabaseContext _context;
        public ReportService(DatabaseContext context)
        {
            _context = context;
        }

        private Task<List<BsonDocument>> AggregateAsync(string collectionName, BsonDocument[] pipeline)
        {
            var collection = _context.Materials.Database.GetCollection<BsonDocument>(collectionName);
            return collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
        }

        public Task<List<BsonDocument>> GetApprovedMaterialsAsync()
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("status", "Approved")),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "users" },
                    { "localField", "uploadedBy" },
                    { "foreignField", "_id" },
                    { "as", "uploader" }
                }),
                new BsonDocument("$unwind", "$uploader"),
                new BsonDocument("$project", new BsonDocument
                {
                    { "Title", "$title" },
                    { "UploadedBy", "$uploader.name" },
                    { "Status", "$status" },
                    { "UploadDate", "$uploadDate" },
                    { "ViewsCount", "$viewsCount" },
                    { "FileLink", "$fileLink" },
                    { "_id", "$_id" }
                })
            };
            return AggregateAsync("materials", pipeline);
        }

        public Task<List<BsonDocument>> GetTrendingAsync()
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("status", "Approved")),
                new BsonDocument("$sort", new BsonDocument("viewsCount", -1)),
                new BsonDocument("$limit", 5),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "courses" },
                    { "localField", "courseId" },
                    { "foreignField", "_id" },
                    { "as", "courseInfo" }
                }),
                new BsonDocument("$unwind", "$courseInfo"),
                new BsonDocument("$project", new BsonDocument
                {
                    { "Title", "$title" },
                    { "Views", "$viewsCount" },
                    { "Course", "$courseInfo.title" },
                    { "Status", "$status" },
                    { "FileLink", "$fileLink" },
                    { "_id", "$_id" }
                })
            };
            return AggregateAsync("materials", pipeline);
        }

        public Task<List<BsonDocument>> GetPendingAsync()
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("status", "Pending")),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "users" },
                    { "localField", "uploadedBy" },
                    { "foreignField", "_id" },
                    { "as", "uploader" }
                }),
                new BsonDocument("$unwind", "$uploader"),
                new BsonDocument("$project", new BsonDocument
                {
                    { "Material", "$title" },
                    { "UploadedAt", "$uploadDate" },
                    { "By", "$uploader.name" },
                    { "FileLink", "$fileLink" },
                    { "_id", "$_id" }
                })
            };
            return AggregateAsync("materials", pipeline);
        }

        public Task<List<BsonDocument>> GetContributorsAsync()
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$uploadedBy" },
                    { "UploadCount", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$sort", new BsonDocument("UploadCount", -1)),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "users" },
                    { "localField", "_id" },
                    { "foreignField", "_id" },
                    { "as", "user" }
                }),
                new BsonDocument("$unwind", "$user"),
                new BsonDocument("$project", new BsonDocument
                {
                    { "Name", "$user.name" },
                    { "Uploads", "$UploadCount" },
                    { "_id", 0 }
                })
            };
            return AggregateAsync("materials", pipeline);
        }

        public Task<List<BsonDocument>> GetSectionsAsync()
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$sectionId" },
                    { "CourseCount", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "sections" },
                    { "localField", "_id" },
                    { "foreignField", "_id" },
                    { "as", "section" }
                }),
                new BsonDocument("$unwind", "$section"),
                new BsonDocument("$project", new BsonDocument
                {
                    { "Section", "$section.sectionName" },
                    { "TotalCourses", "$CourseCount" },
                    { "_id", 0 }
                })
            };
            return AggregateAsync("courses", pipeline);
        }

        public Task<List<BsonDocument>> SearchApprovedMaterialsAsync(BsonRegularExpression regex)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument
                {
                    { "status", "Approved" },
                    { "title", regex }
                }),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "users" },
                    { "localField", "uploadedBy" },
                    { "foreignField", "_id" },
                    { "as", "uploader" }
                }),
                new BsonDocument("$unwind", "$uploader"),
                new BsonDocument("$project", new BsonDocument
                {
                    { "Title", "$title" },
                    { "UploadedBy", "$uploader.name" },
                    { "Status", "$status" },
                    { "UploadDate", "$uploadDate" },
                    { "ViewsCount", "$viewsCount" },
                    { "FileLink", "$fileLink" },
                    { "_id", "$_id" }
                })
            };
            return AggregateAsync("materials", pipeline);
        }
    }
}
