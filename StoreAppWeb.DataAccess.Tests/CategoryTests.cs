using DataAccess.IRepositories;
using DataAccess.Repositories;
using StoreApp.Models;
using StoreAppWeb.DataAccess.Tests.Base;
using Xunit;

namespace StoreAppWeb.DataAccess.Tests
{
    public class CategoryTests : BaseTestData
    {
        public CategoryTests() : base()
        {
        
        }

        [Fact(DisplayName ="CategoryTests - GetAll")]
        public void CategoryRepositoryGetAllTest()
        {
            //Arrange
            Category[] testCategories = GetTestCategories();
            _context.AddRange(testCategories);
            _context.SaveChanges();

            ICategoryRepository categoryRepo = new CategoryRepository(_context);

            //Act
            var result = categoryRepo.GetAll();

            //Assert
            Assert.Equal(3, result.Count());
        }

        [Fact(DisplayName = "CategoryTests - Get - GetByName")]
        public void CategoryRepositoryGetNameTest()
        {
            //Arrange
            Category[] testCategories = GetTestCategories();
            _context.AddRange(testCategories);
            _context.SaveChanges();

            Category expectedResult = testCategories.FirstOrDefault(e => e.Id == 2);

            ICategoryRepository categoryRepo = new CategoryRepository(_context);

            //Act
            var result = categoryRepo.Get(c => c.Id == 2);

            //Assert
            Assert.Equal(expectedResult.Name, result.Name);
        }

        [Fact(DisplayName = "CategoryTests - Add")]
        public void CategoryRepositoryAddTest()
        {
            //Arrange
            Category[] testCategories = GetTestCategories();
            _context.AddRange(testCategories);
            _context.SaveChanges();

            int expectedResult = _context.Categories.Count() + 1;

            ICategoryRepository categoryRepo = new CategoryRepository(_context);

            Category categoryToAdd = new Category { Id = 90, DisplayOrder = 80, Name = "TestName" };

            //Act
            categoryRepo.Add(categoryToAdd);
            _context.SaveChanges();

            int result = _context.Categories.Count();
            Category addedCategory = _context.Categories.FirstOrDefault(c => c.Id == categoryToAdd.Id);

            //Assert
            Assert.Equal(expectedResult, result);
            Assert.Same(categoryToAdd, addedCategory);
        }

        [Fact(DisplayName = "CategoryTests - Delete")]
        public void CategoryRepositoryDeleteTest()
        {
            //Arrange
            Category[] testCategories = GetTestCategories();
            _context.AddRange(testCategories);
            _context.SaveChanges();

            int expectedResult = _context.Categories.Count() - 1;

            ICategoryRepository categoryRepo = new CategoryRepository(_context);

            Category categoryToDelete = _context.Categories.FirstOrDefault();

            //Act
            categoryRepo.Delete(categoryToDelete);
            _context.SaveChanges();

            int result = _context.Categories.Count();
            Category deletedCategory = _context.Categories.FirstOrDefault(c => c.Id == categoryToDelete.Id);

            //Assert
            Assert.Equal(expectedResult, result);
            Assert.Same(null, deletedCategory);
        }

        [Fact(DisplayName = "CategoryTests - Update")]
        public void CategoryRepositoryUpdateTest()
        {
            //Arrange
            Category[] testCategories = GetTestCategories();
            _context.AddRange(testCategories);
            _context.SaveChanges();

            ICategoryRepository categoryRepo = new CategoryRepository(_context);

            Category categoryToUpdate = _context.Categories.FirstOrDefault();
            string expectedCategoryName = categoryToUpdate.Name + "Updated";
            categoryToUpdate.Name = expectedCategoryName;

            //Act
            categoryRepo.Update(categoryToUpdate);
            _context.SaveChanges();
                       
            Category updatedCategory = _context.Categories.FirstOrDefault(c => c.Id == categoryToUpdate.Id);

            //Assert
            Assert.Same(expectedCategoryName, updatedCategory.Name);
        }

        [Fact(DisplayName = "CategoryTests - Delete")]
        public void CategoryRepositoryDeleteFewTest()
        {
            //Arrange
            Category[] testCategories = GetTestCategories();
            _context.AddRange(testCategories);
            _context.SaveChanges();

            int expectedResult = _context.Categories.Count() - 2;

            ICategoryRepository categoryRepo = new CategoryRepository(_context);

            Category[] categoriesToDelete = _context.Categories.Take(2).ToArray();

            //Act
            categoryRepo.DeleteFew(categoriesToDelete);
            _context.SaveChanges();

            int result = _context.Categories.Count();

            //Assert
            Assert.Equal(expectedResult, result);
        }

        #region TestData

        private Category[] GetTestCategories()
        {
            return new Category[]
            {
                new Category { Id = 1, DisplayOrder = 5, Name = "Historical" },
                new Category { Id = 2, DisplayOrder = 9, Name = "Romance" },
                new Category { Id = 3, DisplayOrder = 12, Name = "Thriller" }
            };
        }

        #endregion
    }
}
