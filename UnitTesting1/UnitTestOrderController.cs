//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using WebApplication2.Controllers;
//using Microsoft.EntityFrameworkCore.Infrastructure;
//using Microsoft.Identity.Client;
//using System.Linq.Expressions;
//using WebApplication2.Repository;
//using System.Formats.Asn1;
//using RestuarantBackend.Infrastructure.Entity;

//namespace UnitTesting1
//{
//    public class UnitTestOrderController
//    {

//        //[Fact]
//        //public void OrderController_HealthTest_ReturnString()
//        //{
//        //    //arrange
//        //    var mock = new Mock<IRepo<CartItems>>();
//        //    string x = "cookie"; 
            
//        //    //return the null expection
//        //    mock.Setup(m => m.ReturnString(x)).Returns("cookie");

        
//        //    //act
//        //    var OrderController = new OrderController(null, mock.Object, null);
//        //    //this is a moq- so the bro below does not acutally get sent????
//        //    var expected = OrderController.Healthy("cookie"); 
//        //    Console.WriteLine(expected);
//        //    //assert 
//        //    //Assert.Equal("cookie", expected);
//        //    // Assert.True(expected.Length > 0);
//        //    //ToDoContext _context = new ToDoContext(); 
//        //    CartItemsRepo t = new CartItemsRepo(null); 

//        //    Assert.Throws<NotImplementedException> (() => t.ReturnString("cookie")); 
//        //    //will return nothibg if the test equal each nother
//        //   // Assert.Null(expected); 
     
            
//        //}

//        [Fact]
//        public async Task OrderController_Testing_GetCartItems()
//        {
//            //arrange
//            List<CartItems> cartItems = new List<CartItems>();
//            cartItems.Add(new CartItems { Id = 10, OrderInformationNameonCard = null, item = "nothing", price = "15.75" }); 
     
//            var mock = new Mock<IRepo<CartItems>>();
//            mock.Setup(m =>  m.CartItemsAsync()).ReturnsAsync(cartItems);
           
//            //act
//            var OrderController = new OrderController(null, mock.Object, null,  null);
//            var actual = await OrderController.GetCartItems();
  
//            //assert
//            //Assert.Equal(Id, 10); 
//            Assert.NotNull(actual);
//            Assert.Same(cartItems, actual);
            

//        }


//        [Fact]
//        public async Task OrderController_Testing_PostInformationAsync()
//        {
//            //arrange 
//            var cartItems = new CartItems { Id = 10, OrderInformationNameonCard = null, item = "nothing", price = "15.75" }; 
//            var mock = new Mock<IRepo<CartItems>>();
//            //does not return a type 
//            mock.Setup(m => m.PostItemsAsync(cartItems)).ReturnsAsync(cartItems);

//            //act 
//            var OrderController = new OrderController(null, mock.Object,null,  null); 
//            var acutal = await OrderController.PostInformationAsync(cartItems);

//            //arrange 
//            Assert.NotNull(acutal);
//            Assert.Same(cartItems, acutal); 
//        }


//    }
//}