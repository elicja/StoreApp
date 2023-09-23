using DataAccess.IRepositories;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Models;
using StoreAppWeb.Areas.Admin.Controllers;
using Xunit;

namespace StoreAppWeb.DataAccess.Tests
{
    public class CompanyControllerTests
    {
        IUnitOfWork _unitOfWork;
        ICompanyRepository _companyRepo;
        CompanyController _companyController;

        public CompanyControllerTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _companyRepo = A.Fake<ICompanyRepository>();

            A.CallTo(() => _unitOfWork.CompanyRepo).Returns(_companyRepo);
            _companyController = new CompanyController(_unitOfWork);
            _companyController.TempData = A.Fake<ITempDataDictionary>();
        }

        [Theory(DisplayName = "Get Upsert")]
        [InlineData(0, 0)]
        [InlineData(10, 1)]
        public void GetUpsert_Test(int id, int howManyTimesGet)
        {
            //Arrange

            //Act
            _companyController.Upsert(id);

            //Assert
            A.CallTo(() => _companyRepo.Get(x => true, null, false)).WithAnyArguments().MustHaveHappened(howManyTimesGet, Times.Exactly);
        }

        [Theory(DisplayName = "Post upsert")]
        [InlineData(0,1,0)]
        [InlineData(10,0,1)]
        public void PostUpsert_Test(int id, int howManyTimesAdd, int howManyTimesUpdate)
        {
            //Arrange
            Company testCompany = new Company { Id = id };

            //Act
            _companyController.Upsert(testCompany);

            //Assert
            A.CallTo(() => _companyRepo.Add(A<Company>._)).MustHaveHappened(howManyTimesAdd,Times.Exactly);
            A.CallTo(() => _companyRepo.Update(A<Company>._)).MustHaveHappened(howManyTimesUpdate, Times.Exactly);
            A.CallTo(() => _unitOfWork.Save()).MustHaveHappenedOnceExactly();
        }       

        [Fact(DisplayName ="Delete - data exists")]
        public void Delete_DataExists_Test()
        {
            //Arrange
            A.CallTo(() => _companyRepo.Get(x => true, null, false)).WithAnyArguments().Returns(new Company());

            //Act
            _companyController.Delete(10);

            //Assert
            A.CallTo(() => _companyRepo.Delete(A<Company>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _unitOfWork.Save()).MustHaveHappenedOnceExactly();
        }

        [Fact(DisplayName = "Delete - data does not exist")]
        public void Delete_DataDoesNotExist_Test()
        {
            //Arrange
            A.CallTo(() => _companyRepo.Get(x => true, null, false)).WithAnyArguments().Returns(null);

            //Act
            _companyController.Delete(null);

            //Assert
            A.CallTo(() => _companyRepo.Delete(A<Company>._)).MustNotHaveHappened();
            A.CallTo(() => _unitOfWork.Save()).MustNotHaveHappened();
        }
    }
}
