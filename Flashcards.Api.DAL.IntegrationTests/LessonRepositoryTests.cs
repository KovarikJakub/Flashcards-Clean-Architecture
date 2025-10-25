using Flashcards.Api.DAL.Common.Entities;

namespace Flashcards.Api.DAL.IntegrationTests
{
    public class LessonRepositoryTests
    {
        private readonly IFakeDatabase fakeDb;

        public LessonRepositoryTests()
        {
            fakeDb = new InMemoryFakeDatabase();
        }

        private bool AreLessonsEqual(LessonEntity left, LessonEntity right)
        {
            return left.Id == right.Id &&
                   left.User.Id == right.User.Id &&
                   left.CardCollection.Id == right.CardCollection.Id &&
                   left.Start == right.Start &&
                   left.Seen == right.Seen &&
                   left.Correct == right.Correct &&
                   left.Wrong == right.Wrong;
        }

        [Fact]
        public void GetById_Existing_Lesson()
        {
            // Arrange
            var repo = fakeDb.GetLessonRepository();
            var requestedId = fakeDb.LessonGuids[0];

            // Act
            var lesson = repo.GetById(requestedId);

            // Assert
            Assert.NotNull(lesson);
            Assert.Equal(requestedId, lesson.Id);

            var expectedLesson = fakeDb.GetLessonById(requestedId);
            Assert.True(AreLessonsEqual(lesson, expectedLesson));
        }

        [Fact]
        public void GetById_NonExisting_Lesson()
        {
            // Arrange
            var repo = fakeDb.GetLessonRepository();
            var nonExistingId = Guid.NewGuid();

            // Act
            var lesson = repo.GetById(nonExistingId);

            // Assert
            Assert.Null(lesson);
        }

        [Fact]
        public void GetAll_Returns_All_Lessons()
        {
            // Arrange
            var repo = fakeDb.GetLessonRepository();

            // Act
            var lessons = repo.GetAll();

            // Assert
            Assert.Equal(fakeDb.LessonGuids.Count, lessons.Count);
            foreach (var l in lessons)
            {
                var expected = fakeDb.GetLessonById(l.Id);
                Assert.True(AreLessonsEqual(l, expected));
            }
        }

        [Fact]
        public void Insert_New_Lesson()
        {
            // Arrange
            var repo = fakeDb.GetLessonRepository();
            var newLesson = new LessonEntity
            {
                Id = Guid.NewGuid(),
                User = fakeDb.GetUserById(fakeDb.UserGuids[0]),
                CardCollection = fakeDb.GetCardCollectionById(fakeDb.CardCollectionGuids[0]),
                Start = new DateTime(2025, 3, 1, 10, 0, 0),
                Seen = 1,
                Correct = 1,
                Wrong = 0
            };

            // Act
            var newId = repo.Insert(newLesson);

            // Assert
            var inserted = fakeDb.GetLessonById(newId);
            Assert.NotNull(inserted);
            Assert.True(AreLessonsEqual(inserted, newLesson));
        }

        [Fact]
        public void Update_Existing_Lesson()
        {
            // Arrange
            var repo = fakeDb.GetLessonRepository();
            var lessonId = fakeDb.LessonGuids[0];
            var lesson = repo.GetById(lessonId);
            lesson.Seen = 10;
            lesson.Correct = 8;

            // Act
            repo.Update(lesson);

            // Assert
            var updated = fakeDb.GetLessonById(lessonId);
            Assert.Equal(10u, updated.Seen);
            Assert.Equal(8u, updated.Correct);
        }

        [Fact]
        public void Delete_Existing_Lesson()
        {
            // Arrange
            var repo = fakeDb.GetLessonRepository();
            var idToRemove = fakeDb.LessonGuids[0];

            // Act
            repo.Remove(idToRemove);

            // Assert
            Assert.Null(fakeDb.GetLessonById(idToRemove));
        }
    }
}
