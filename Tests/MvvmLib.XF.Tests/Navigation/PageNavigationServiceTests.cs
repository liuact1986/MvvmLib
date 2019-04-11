using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;
using MvvmLib.XF.Tests.Views;
using Xamarin.Forms;

namespace MvvmLib.XF.Tests
{

    public class FakeNavigationStrategy : INavigationStrategy
    {
        public List<Page> FakeModalStack = new List<Page>();
        public IReadOnlyList<Page> ModalStack => FakeModalStack;

        public List<Page> FakeNavigationStack = new List<Page>();
        public IReadOnlyList<Page> NavigationStack => FakeNavigationStack;

        public bool PopIsCalled { get; private set; }
        public bool PopModalIsCalled { get; private set; }
        public bool PopToRootIsCalled { get; private set; }
        public bool PushIsCalled { get; private set; }
        public bool PushModalIsCalled { get; private set; }

        public event EventHandler<NavigationEventArgs> Popped;

        public void Reset()
        {
            PopIsCalled = false;
            PopModalIsCalled = false;
            PopToRootIsCalled = false;
            PushIsCalled = false;
            PushModalIsCalled = false;
        }

        // push 

        public Task PushAsync(Page page)
        {
            PushIsCalled = true;
            FakeNavigationStack.Add(page);

            return Task.FromResult<object>(null);
        }

        public Task PushAsync(Page page, bool animated)
        {
            PushIsCalled = true;
            FakeNavigationStack.Add(page);

            return Task.FromResult<object>(null);
        }

        public Task PushModalAsync(Page page)
        {
            PushModalIsCalled = true;
            FakeModalStack.Add(page);

            return Task.FromResult<object>(null);
        }

        public Task PushModalAsync(Page page, bool animated)
        {
            PushModalIsCalled = true;
            FakeModalStack.Add(page);

            return Task.FromResult<object>(null);
        }


        public Task PopAsync()
        {
            PopIsCalled = true;
            FakeNavigationStack.RemoveAt(FakeNavigationStack.Count - 1);

            return Task.FromResult<object>(null);
        }

        public Task PopAsync(bool animated)
        {
            PopIsCalled = true;
            FakeNavigationStack.RemoveAt(FakeNavigationStack.Count - 1);

            return Task.FromResult<object>(null);
        }

        public Task PopModalAsync()
        {
            PopModalIsCalled = true;
            FakeModalStack.RemoveAt(FakeModalStack.Count - 1);

            return Task.FromResult<object>(null);
        }

        public Task PopModalAsync(bool animated)
        {
            PopModalIsCalled = true;
            FakeModalStack.RemoveAt(FakeModalStack.Count - 1);

            return Task.FromResult<object>(null);
        }

        public Task PopToRootAsync()
        {
            PopToRootIsCalled = true;

            FakeNavigationStack.RemoveRange(1, FakeNavigationStack.Count - 1);

            return Task.FromResult<object>(null);
        }

        public Task PopToRootAsync(bool animated)
        {
            PopToRootIsCalled = true;

            FakeNavigationStack.RemoveRange(1, FakeNavigationStack.Count - 1);

            return Task.FromResult<object>(null);
        }


        public void RemovePage(Page page)
        {

        }
    }



    [TestClass]
    public class PageNavigationServiceTests
    {
        //public PageNavigationService GetService()
        //{
        //    return new PageNavigationService(new FakeNavigationStrategy());
        //}

        [TestMethod]
        public async Task Push()
        {
            var s = new FakeNavigationStrategy();
            var service = new PageNavigationService(s);

            await service.PushAsync(typeof(PageA));
            Assert.IsTrue(s.PushIsCalled);
            s.Reset();

            await service.PushAsync(typeof(PageA), animated: true);
            Assert.IsTrue(s.PushIsCalled);
            s.Reset();

            await service.PushAsync(typeof(PageA), parameter: 10);
            Assert.IsTrue(s.PushIsCalled);
            s.Reset();

            await service.PushAsync(typeof(PageA), parameter: 10, animated: true);
            Assert.IsTrue(s.PushIsCalled);
            s.Reset();

            // push modal

        }

        [TestMethod]
        public async Task Push_With_NavigationPage()
        {
            var p = new NavigationPage();

            Assert.AreEqual(0, p.Navigation.NavigationStack.Count);

            await p.Navigation.PushAsync(new HomePage());

            // - 0 : HomePage
            Assert.AreEqual(1, p.Navigation.NavigationStack.Count);
            Assert.AreEqual(typeof(HomePage), p.Navigation.NavigationStack.LastOrDefault().GetType());

            // - 0 : HomePage
            // - 1 : PageA
            await p.Navigation.PushAsync(new PageA());
            Assert.AreEqual(2, p.Navigation.NavigationStack.Count);
            Assert.AreEqual(typeof(PageA), p.Navigation.NavigationStack.LastOrDefault().GetType());

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.Navigation.PushAsync(new PageB());
            Assert.AreEqual(3, p.Navigation.NavigationStack.Count);
            Assert.AreEqual(typeof(PageB), p.Navigation.NavigationStack.LastOrDefault().GetType());

            // can Go back if NavigationStack count > 1
            // - 0 : HomePage (count 1) No
            // - 1 : PageA (count 2) Yes

        }

        [TestMethod]
        public async Task Push_With_NavigationService()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            Assert.AreEqual(0, s.NavigationStack.Count);

            await p.PushAsync(typeof(HomePage));

            // - 0 : HomePage
            Assert.AreEqual(1, s.NavigationStack.Count);
            Assert.AreEqual(typeof(HomePage), s.NavigationStack.LastOrDefault().GetType());
            Assert.AreEqual(null, p.PreviousPage);
            Assert.AreEqual(typeof(HomePage), p.CurrentPage.GetType());
            Assert.AreEqual(typeof(HomePage), p.RootPage.GetType());
            Assert.AreEqual(false, p.CanPop);

            // - 0 : HomePage
            // - 1 : PageA
            await p.PushAsync(typeof(PageA));
            Assert.AreEqual(2, s.NavigationStack.Count);
            Assert.AreEqual(typeof(HomePage), p.PreviousPage.GetType());
            Assert.AreEqual(typeof(PageA), s.NavigationStack.LastOrDefault().GetType());
            Assert.AreEqual(typeof(PageA), p.CurrentPage.GetType());
            Assert.AreEqual(typeof(HomePage), p.RootPage.GetType());
            Assert.AreEqual(true, p.CanPop);

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushAsync(typeof(PageB));
            Assert.AreEqual(3, s.NavigationStack.Count);
            Assert.AreEqual(typeof(PageA), p.PreviousPage.GetType());
            Assert.AreEqual(typeof(PageB), s.NavigationStack.LastOrDefault().GetType());
            Assert.AreEqual(typeof(PageB), p.CurrentPage.GetType());
            Assert.AreEqual(typeof(HomePage), p.RootPage.GetType());
            Assert.AreEqual(true, p.CanPop);

            // can Go back if NavigationStack count > 1
            // - 0 : HomePage (count 1) No
            // - 1 : PageA (count 2) Yes

        }

        [TestMethod]
        public async Task PushModal_With_NavigationPage()
        {
            var p = new NavigationPage();

            Assert.AreEqual(0, p.Navigation.ModalStack.Count);

            await p.Navigation.PushModalAsync(new HomePage());

            // - 0 : HomePage
            Assert.AreEqual(1, p.Navigation.ModalStack.Count);
            Assert.AreEqual(typeof(HomePage), p.Navigation.ModalStack.LastOrDefault().GetType());

            // - 0 : HomePage
            // - 1 : PageA
            await p.Navigation.PushModalAsync(new PageA());
            Assert.AreEqual(2, p.Navigation.ModalStack.Count);
            Assert.AreEqual(typeof(PageA), p.Navigation.ModalStack.LastOrDefault().GetType());

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.Navigation.PushModalAsync(new PageB());
            Assert.AreEqual(3, p.Navigation.ModalStack.Count);
            Assert.AreEqual(typeof(PageB), p.Navigation.ModalStack.LastOrDefault().GetType());

            Assert.AreEqual(0, p.Navigation.NavigationStack.Count);

            // can Go back if ModalStack count > 1
            // - 0 : HomePage (count 1) No
            // - 1 : PageA (count 2) Yes

        }

        [TestMethod]
        public async Task PushModal_With_NavigationService()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            Assert.AreEqual(0, s.ModalStack.Count);

            await p.PushModalAsync(typeof(HomePage));

            // - 0 : HomePage
            Assert.AreEqual(1, s.ModalStack.Count);
            Assert.AreEqual(typeof(HomePage), s.ModalStack.LastOrDefault().GetType());
            Assert.AreEqual(null, p.PreviousModalPage);
            Assert.AreEqual(typeof(HomePage), p.CurrentModalPage.GetType());
            Assert.AreEqual(true, p.CanPopModal); // <=

            // - 0 : HomePage
            // - 1 : PageA
            await p.PushModalAsync(typeof(PageA));
            Assert.AreEqual(2, s.ModalStack.Count);
            Assert.AreEqual(typeof(HomePage), p.PreviousModalPage.GetType());
            Assert.AreEqual(typeof(PageA), s.ModalStack.LastOrDefault().GetType());
            Assert.AreEqual(typeof(PageA), p.CurrentModalPage.GetType());
            Assert.AreEqual(true, p.CanPopModal);

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushModalAsync(typeof(PageB));
            Assert.AreEqual(3, s.ModalStack.Count);
            Assert.AreEqual(typeof(PageA), p.PreviousModalPage.GetType());
            Assert.AreEqual(typeof(PageB), s.ModalStack.LastOrDefault().GetType());
            Assert.AreEqual(typeof(PageB), p.CurrentModalPage.GetType());
            Assert.AreEqual(true, p.CanPopModal);

            Assert.AreEqual(0, p.NavigationStack.Count);

            // can Go back if ModalStack count > 0
            // - 0 : HomePage (count 1) No
            // - 1 : PageA (count 2) Yes

        }

        [TestMethod]
        public async Task Pop_With_NavigationPage()
        {
            var p = new NavigationPage();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.Navigation.PushAsync(new HomePage());
            await p.Navigation.PushAsync(new PageA());
            await p.Navigation.PushAsync(new PageB());

            Assert.AreEqual(3, p.Navigation.NavigationStack.Count);
            Assert.AreEqual(typeof(HomePage), p.Navigation.NavigationStack.ElementAt(0).GetType());
            Assert.AreEqual(typeof(PageA), p.Navigation.NavigationStack.ElementAt(1).GetType());
            Assert.AreEqual(typeof(PageB), p.Navigation.NavigationStack.ElementAt(2).GetType());


            await p.Navigation.PopAsync();

            Assert.AreEqual(2, p.Navigation.NavigationStack.Count);
            Assert.AreEqual(typeof(HomePage), p.Navigation.NavigationStack.ElementAt(0).GetType());
            Assert.AreEqual(typeof(PageA), p.Navigation.NavigationStack.ElementAt(1).GetType());

            await p.Navigation.PopAsync();

            Assert.AreEqual(1, p.Navigation.NavigationStack.Count);
            Assert.AreEqual(typeof(HomePage), p.Navigation.NavigationStack.ElementAt(0).GetType());

            await p.Navigation.PopAsync();

            // cannot pop if Navigationstack < 1

            Assert.AreEqual(1, p.Navigation.NavigationStack.Count);
            Assert.AreEqual(typeof(HomePage), p.Navigation.NavigationStack.ElementAt(0).GetType());
        }

        [TestMethod]
        public async Task Pop_With_NavigationService()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushAsync(typeof(HomePage));
            await p.PushAsync(typeof(PageA));
            await p.PushAsync(typeof(PageB));

            Assert.AreEqual(3, p.NavigationStack.Count);
            Assert.AreEqual(3, s.NavigationStack.Count);
            Assert.AreEqual(typeof(HomePage), p.NavigationStack.ElementAt(0).GetType());
            Assert.AreEqual(typeof(PageA), p.NavigationStack.ElementAt(1).GetType());
            Assert.AreEqual(typeof(PageB), p.NavigationStack.ElementAt(2).GetType());


            await p.PopAsync();

            Assert.AreEqual(2, p.NavigationStack.Count);
            Assert.AreEqual(typeof(HomePage), p.NavigationStack.ElementAt(0).GetType());
            Assert.AreEqual(typeof(PageA), p.NavigationStack.ElementAt(1).GetType());

            await p.PopAsync();

            Assert.AreEqual(1, p.NavigationStack.Count);
            Assert.AreEqual(typeof(HomePage), p.NavigationStack.ElementAt(0).GetType());

            await p.PopAsync();

            // cannot pop if Navigationstack < 1

            Assert.AreEqual(1, p.NavigationStack.Count);
            Assert.AreEqual(typeof(HomePage), p.NavigationStack.ElementAt(0).GetType());
            Assert.AreEqual(false, p.CanPop);
        }

        [TestMethod]
        public async Task PopModal_With_NavigationPage()
        {
            var p = new NavigationPage();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.Navigation.PushModalAsync(new HomePage());
            await p.Navigation.PushModalAsync(new PageA());
            await p.Navigation.PushModalAsync(new PageB());

            Assert.AreEqual(3, p.Navigation.ModalStack.Count);
            Assert.AreEqual(typeof(HomePage), p.Navigation.ModalStack.ElementAt(0).GetType());
            Assert.AreEqual(typeof(PageA), p.Navigation.ModalStack.ElementAt(1).GetType());
            Assert.AreEqual(typeof(PageB), p.Navigation.ModalStack.ElementAt(2).GetType());


            await p.Navigation.PopModalAsync();

            Assert.AreEqual(2, p.Navigation.ModalStack.Count);
            Assert.AreEqual(typeof(HomePage), p.Navigation.ModalStack.ElementAt(0).GetType());
            Assert.AreEqual(typeof(PageA), p.Navigation.ModalStack.ElementAt(1).GetType());

            await p.Navigation.PopModalAsync();

            Assert.AreEqual(1, p.Navigation.ModalStack.Count);
            Assert.AreEqual(typeof(HomePage), p.Navigation.ModalStack.ElementAt(0).GetType());

            await p.Navigation.PopModalAsync();

            // cannot pop if Modalstack < 0

            Assert.AreEqual(0, p.Navigation.ModalStack.Count);
        }

        [TestMethod]
        public async Task PopModal_With_NavigationService()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushModalAsync(typeof(HomePage));
            await p.PushModalAsync(typeof(PageA));
            await p.PushModalAsync(typeof(PageB));

            Assert.AreEqual(3, p.ModalStack.Count);
            Assert.AreEqual(3, s.ModalStack.Count);
            Assert.AreEqual(typeof(HomePage), p.ModalStack.ElementAt(0).GetType());
            Assert.AreEqual(typeof(PageA), p.ModalStack.ElementAt(1).GetType());
            Assert.AreEqual(typeof(PageB), p.ModalStack.ElementAt(2).GetType());

            await p.PopModalAsync();

            Assert.AreEqual(2, p.ModalStack.Count);
            Assert.AreEqual(typeof(HomePage), p.ModalStack.ElementAt(0).GetType());
            Assert.AreEqual(typeof(PageA), p.ModalStack.ElementAt(1).GetType());

            await p.PopModalAsync();

            Assert.AreEqual(1, p.ModalStack.Count);
            Assert.AreEqual(typeof(HomePage), p.ModalStack.ElementAt(0).GetType());

            await p.PopModalAsync();

            // cannot pop if Navigationstack < 0

            Assert.AreEqual(0, p.ModalStack.Count);
            Assert.AreEqual(false, p.CanPopModal);
        }

        [TestMethod]
        public async Task PopToRoot_With_NavigationPage()
        {
            var p = new NavigationPage();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.Navigation.PushAsync(new HomePage());
            await p.Navigation.PushAsync(new PageA());
            await p.Navigation.PushAsync(new PageB());

            Assert.AreEqual(3, p.Navigation.NavigationStack.Count);

            await p.Navigation.PopToRootAsync();

            Assert.AreEqual(1, p.Navigation.NavigationStack.Count);
            Assert.AreEqual(typeof(HomePage), p.Navigation.NavigationStack.ElementAt(0).GetType());
        }

        [TestMethod]
        public async Task PopToRoot_With_NavigationService()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushAsync(typeof(HomePage));
            await p.PushAsync(typeof(PageA));
            await p.PushAsync(typeof(PageB));

            Assert.AreEqual(3, p.NavigationStack.Count);
            Assert.AreEqual(typeof(HomePage), p.RootPage.GetType());

            await p.PopToRootAsync();

            Assert.AreEqual(1, p.NavigationStack.Count);
            Assert.AreEqual(typeof(HomePage), p.NavigationStack.ElementAt(0).GetType());
        }

        [TestMethod]
        public async Task ViewModel_Is_Notified_On_Push()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            MyViewModel.Reset();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushAsync(typeof(PageWithViewModel), "p");

            Assert.AreEqual(true, MyViewModel.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModel.OnNavigatedToCalled);
            Assert.AreEqual("p", MyViewModel.OnNavigatingToParameter);
            Assert.AreEqual("p", MyViewModel.OnNavigatedToParameter);
            Assert.AreEqual(false, MyViewModel.OnNavigatingFromCalled);

            await p.PushAsync(typeof(PageA));
            Assert.AreEqual(true, MyViewModel.OnNavigatingFromCalled);
        }

        [TestMethod]
        public async Task ViewModel_Is_Notified_On_PushModal()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            MyViewModel.Reset();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushModalAsync(typeof(PageWithViewModel), "p");

            Assert.AreEqual(true, MyViewModel.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModel.OnNavigatedToCalled);
            Assert.AreEqual("p", MyViewModel.OnNavigatingToParameter);
            Assert.AreEqual("p", MyViewModel.OnNavigatedToParameter);
            Assert.AreEqual(false, MyViewModel.OnNavigatingFromCalled);

            await p.PushModalAsync(typeof(PageA));
            Assert.AreEqual(true, MyViewModel.OnNavigatingFromCalled);
        }

        [TestMethod]
        public async Task Push_With_Guard_On_ViewModel()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            MyViewModelWithGuard.Reset();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushAsync(typeof(PageWithViewModelAndGuard), "p");

            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("p", MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("p", MyViewModelWithGuard.OnNavigatedToParameter);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            await p.PushAsync(typeof(PageA));
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingFromCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);

            // Can activate
            MyViewModelWithGuard.Reset();
            MyViewModelWithGuard.CanActivate = false;
            await p.PushAsync(typeof(PageWithViewModelAndGuard), "p2");
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p2", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatedToParameter);

            MyViewModelWithGuard.Reset();
            await p.PushAsync(typeof(PageWithViewModelAndGuard), "p3");
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p3", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("p3", MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("p3", MyViewModelWithGuard.OnNavigatedToParameter);

            MyViewModelWithGuard.CanDeactivate = false;
            await p.PushAsync(typeof(PageA));
            Assert.AreEqual(false, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            MyViewModelWithGuard.CanDeactivate = true;
            await p.PushAsync(typeof(PageA));
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingFromCalled);
        }

        [TestMethod]
        public async Task Push_With_Guard_On_Page()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            PageWithGuard.Reset();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushAsync(typeof(PageWithGuard), "p");

            Assert.AreEqual(true, PageWithGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuard.CanActivate);
            Assert.AreEqual("p", PageWithGuard.CanActivateParameter);

            Assert.AreEqual(true, PageWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, PageWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("p", PageWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("p", PageWithGuard.OnNavigatedToParameter);
            Assert.AreEqual(false, PageWithGuard.OnNavigatingFromCalled);

            await p.PushAsync(typeof(PageA));
            Assert.AreEqual(true, PageWithGuard.OnNavigatingFromCalled);
            Assert.AreEqual(true, PageWithGuard.CanDeactivate);
            Assert.AreEqual(true, PageWithGuard.CanDeactivateCalled);

            // Can activate
            PageWithGuard.Reset();
            PageWithGuard.CanActivate = false;
            await p.PushAsync(typeof(PageWithGuard), "p2");
            Assert.AreEqual(true, PageWithGuard.CanActivateCalled);
            Assert.AreEqual(false, PageWithGuard.CanActivate);
            Assert.AreEqual("p2", PageWithGuard.CanActivateParameter);

            Assert.AreEqual(false, PageWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, PageWithGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, PageWithGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, PageWithGuard.OnNavigatedToParameter);

            PageWithGuard.Reset();
            await p.PushAsync(typeof(PageWithGuard), "p3");
            Assert.AreEqual(true, PageWithGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuard.CanActivate);
            Assert.AreEqual("p3", PageWithGuard.CanActivateParameter);

            Assert.AreEqual(true, PageWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, PageWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("p3", PageWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("p3", PageWithGuard.OnNavigatedToParameter);

            PageWithGuard.CanDeactivate = false;
            await p.PushAsync(typeof(PageA));
            Assert.AreEqual(false, PageWithGuard.CanDeactivate);
            Assert.AreEqual(true, PageWithGuard.CanDeactivateCalled);
            Assert.AreEqual(false, PageWithGuard.OnNavigatingFromCalled);

            PageWithGuard.CanDeactivate = true;
            await p.PushAsync(typeof(PageA));
            Assert.AreEqual(true, PageWithGuard.CanDeactivate);
            Assert.AreEqual(true, PageWithGuard.CanDeactivateCalled);
            Assert.AreEqual(true, PageWithGuard.OnNavigatingFromCalled);
        }

        [TestMethod]
        public async Task Push_With_Guard_On_Page_And_ViewModel()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushAsync(typeof(PageWithGuardAndViewModelGuard), "p");

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("p", PageWithGuardAndViewModelGuard.CanActivateParameter);

            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatedToCalled);
            Assert.AreEqual("p", PageWithGuardAndViewModelGuard.OnNavigatingToParameter);
            Assert.AreEqual("p", PageWithGuardAndViewModelGuard.OnNavigatedToParameter);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("p", MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("p", MyViewModelWithGuard.OnNavigatedToParameter);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            await p.PushAsync(typeof(PageA));
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivate);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivateCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingFromCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            // Can activate
            PageWithGuardAndViewModelGuard.CanActivate = false;
            await p.PushAsync(typeof(PageWithGuardAndViewModelGuard), "p2");

            // page block

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("p2", PageWithGuardAndViewModelGuard.CanActivateParameter);

            Assert.AreEqual(false, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual(null, MyViewModelWithGuard.CanActivateParameter);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            // Can activate
            MyViewModelWithGuard.CanActivate = false;
            await p.PushAsync(typeof(PageWithGuardAndViewModelGuard), "p2");

            // view model block

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("p2", PageWithGuardAndViewModelGuard.CanActivateParameter);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatedToCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p2", MyViewModelWithGuard.CanActivateParameter);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatedToCalled);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            await p.PushAsync(typeof(PageWithGuardAndViewModelGuard), "p3");

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("p3", PageWithGuardAndViewModelGuard.CanActivateParameter);

            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p3", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatedToCalled);
            Assert.AreEqual("p3", PageWithGuardAndViewModelGuard.OnNavigatingToParameter);
            Assert.AreEqual("p3", PageWithGuardAndViewModelGuard.OnNavigatedToParameter);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("p3", MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("p3", MyViewModelWithGuard.OnNavigatedToParameter);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            // Can Deactivate

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            PageWithGuardAndViewModelGuard.CanDeactivate = false;

            // page block

            await p.PushAsync(typeof(PageA));

            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.CanDeactivate);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivateCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(false, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            // viewmodel block

            MyViewModelWithGuard.CanDeactivate = false;

            await p.PushAsync(typeof(PageA));
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivate);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivateCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(false, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            await p.PushAsync(typeof(PageA));
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivate);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingFromCalled);
        }


        [TestMethod]
        public async Task PushModal_With_Guard_On_ViewModel()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            MyViewModelWithGuard.Reset();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushModalAsync(typeof(PageWithViewModelAndGuard), "p");

            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("p", MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("p", MyViewModelWithGuard.OnNavigatedToParameter);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            await p.PushModalAsync(typeof(PageA));
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingFromCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);

            // Can activate
            MyViewModelWithGuard.Reset();
            MyViewModelWithGuard.CanActivate = false;
            await p.PushModalAsync(typeof(PageWithViewModelAndGuard), "p2");
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p2", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatedToParameter);

            MyViewModelWithGuard.Reset();
            await p.PushModalAsync(typeof(PageWithViewModelAndGuard), "p3");
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p3", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("p3", MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("p3", MyViewModelWithGuard.OnNavigatedToParameter);

            MyViewModelWithGuard.CanDeactivate = false;
            await p.PushModalAsync(typeof(PageA));
            Assert.AreEqual(false, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            MyViewModelWithGuard.CanDeactivate = true;
            await p.PushModalAsync(typeof(PageA));
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingFromCalled);
        }

        [TestMethod]
        public async Task PushModal_With_Guard_On_Page()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            PageWithGuard.Reset();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushModalAsync(typeof(PageWithGuard), "p");

            Assert.AreEqual(true, PageWithGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuard.CanActivate);
            Assert.AreEqual("p", PageWithGuard.CanActivateParameter);

            Assert.AreEqual(true, PageWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, PageWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("p", PageWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("p", PageWithGuard.OnNavigatedToParameter);
            Assert.AreEqual(false, PageWithGuard.OnNavigatingFromCalled);

            await p.PushModalAsync(typeof(PageA));
            Assert.AreEqual(true, PageWithGuard.OnNavigatingFromCalled);
            Assert.AreEqual(true, PageWithGuard.CanDeactivate);
            Assert.AreEqual(true, PageWithGuard.CanDeactivateCalled);

            // Can activate
            PageWithGuard.Reset();
            PageWithGuard.CanActivate = false;
            await p.PushModalAsync(typeof(PageWithGuard), "p2");
            Assert.AreEqual(true, PageWithGuard.CanActivateCalled);
            Assert.AreEqual(false, PageWithGuard.CanActivate);
            Assert.AreEqual("p2", PageWithGuard.CanActivateParameter);

            Assert.AreEqual(false, PageWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, PageWithGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, PageWithGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, PageWithGuard.OnNavigatedToParameter);

            PageWithGuard.Reset();
            await p.PushModalAsync(typeof(PageWithGuard), "p3");
            Assert.AreEqual(true, PageWithGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuard.CanActivate);
            Assert.AreEqual("p3", PageWithGuard.CanActivateParameter);

            Assert.AreEqual(true, PageWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, PageWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("p3", PageWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("p3", PageWithGuard.OnNavigatedToParameter);

            PageWithGuard.CanDeactivate = false;
            await p.PushModalAsync(typeof(PageA));
            Assert.AreEqual(false, PageWithGuard.CanDeactivate);
            Assert.AreEqual(true, PageWithGuard.CanDeactivateCalled);
            Assert.AreEqual(false, PageWithGuard.OnNavigatingFromCalled);

            PageWithGuard.CanDeactivate = true;
            await p.PushModalAsync(typeof(PageA));
            Assert.AreEqual(true, PageWithGuard.CanDeactivate);
            Assert.AreEqual(true, PageWithGuard.CanDeactivateCalled);
            Assert.AreEqual(true, PageWithGuard.OnNavigatingFromCalled);
        }

        [TestMethod]
        public async Task PushModal_With_Guard_On_Page_And_ViewModel()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushModalAsync(typeof(PageWithGuardAndViewModelGuard), "p");

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("p", PageWithGuardAndViewModelGuard.CanActivateParameter);

            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatedToCalled);
            Assert.AreEqual("p", PageWithGuardAndViewModelGuard.OnNavigatingToParameter);
            Assert.AreEqual("p", PageWithGuardAndViewModelGuard.OnNavigatedToParameter);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("p", MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("p", MyViewModelWithGuard.OnNavigatedToParameter);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            await p.PushModalAsync(typeof(PageA));
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivate);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivateCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingFromCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            // Can activate
            PageWithGuardAndViewModelGuard.CanActivate = false;
            await p.PushModalAsync(typeof(PageWithGuardAndViewModelGuard), "p2");

            // page block

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("p2", PageWithGuardAndViewModelGuard.CanActivateParameter);

            Assert.AreEqual(false, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual(null, MyViewModelWithGuard.CanActivateParameter);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            // Can activate
            MyViewModelWithGuard.CanActivate = false;
            await p.PushModalAsync(typeof(PageWithGuardAndViewModelGuard), "p2");

            // view model block

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("p2", PageWithGuardAndViewModelGuard.CanActivateParameter);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatedToCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p2", MyViewModelWithGuard.CanActivateParameter);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatedToCalled);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            await p.PushModalAsync(typeof(PageWithGuardAndViewModelGuard), "p3");

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("p3", PageWithGuardAndViewModelGuard.CanActivateParameter);

            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p3", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatedToCalled);
            Assert.AreEqual("p3", PageWithGuardAndViewModelGuard.OnNavigatingToParameter);
            Assert.AreEqual("p3", PageWithGuardAndViewModelGuard.OnNavigatedToParameter);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("p3", MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("p3", MyViewModelWithGuard.OnNavigatedToParameter);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            // Can Deactivate

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            PageWithGuardAndViewModelGuard.CanDeactivate = false;

            // page block

            await p.PushModalAsync(typeof(PageA));

            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.CanDeactivate);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivateCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(false, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            // viewmodel block

            MyViewModelWithGuard.CanDeactivate = false;

            await p.PushModalAsync(typeof(PageA));
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivate);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivateCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(false, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            await p.PushModalAsync(typeof(PageA));
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivate);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingFromCalled);
        }

        [TestMethod]
        public async Task ViewModel_Is_Notified_On_Pop()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            MyViewModel.Reset();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushAsync(typeof(PageWithViewModel), "p");

            await p.PushAsync(typeof(PageA), "p");

            await p.PopAsync("px");

            Assert.AreEqual(true, MyViewModel.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModel.OnNavigatedToCalled);
            Assert.AreEqual("px", MyViewModel.OnNavigatingToParameter);
            Assert.AreEqual("px", MyViewModel.OnNavigatedToParameter);
            Assert.AreEqual(true, MyViewModel.OnNavigatingFromCalled);
        }

        [TestMethod]
        public async Task ViewModel_Is_Notified_On_PopModal()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            MyViewModel.Reset();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushModalAsync(typeof(PageWithViewModel), "p");

            await p.PushModalAsync(typeof(PageA), "p");

            await p.PopModalAsync("px");

            Assert.AreEqual(true, MyViewModel.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModel.OnNavigatedToCalled);
            Assert.AreEqual("px", MyViewModel.OnNavigatingToParameter);
            Assert.AreEqual("px", MyViewModel.OnNavigatedToParameter);
            Assert.AreEqual(true, MyViewModel.OnNavigatingFromCalled);
        }

        [TestMethod]
        public async Task Pop_With_Guard_On_ViewModel()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            MyViewModelWithGuard.Reset();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushAsync(typeof(PageWithViewModelAndGuard), "p");

            await p.PushAsync(typeof(PageA));

            await p.PopAsync("px");

            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("px", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("px", MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("px", MyViewModelWithGuard.OnNavigatedToParameter);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingFromCalled);


            MyViewModelWithGuard.Reset();
            MyViewModelWithGuard.CanActivate = false;
            await p.PushAsync(typeof(PageA));

            await p.PopAsync("p2");

            // Can activate
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p2", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatedToParameter);

            // can deactivate

            MyViewModelWithGuard.Reset();
            MyViewModelWithGuard.CanDeactivate = false;

            await p.PushAsync(typeof(PageA));

            await p.PushAsync(typeof(PageWithViewModelAndGuard), "p");

            await p.PopAsync("p3");

            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            MyViewModelWithGuard.Reset();

            await p.PopAsync("p3");
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingFromCalled);

        }

        [TestMethod]
        public async Task Pop_With_Guard_On_Page()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            PageWithGuard.Reset();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushAsync(typeof(PageWithGuard), "p");

            await p.PushAsync(typeof(PageA));

            await p.PopAsync("px");

            Assert.AreEqual(true, PageWithGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuard.CanActivate);
            Assert.AreEqual("px", PageWithGuard.CanActivateParameter);

            Assert.AreEqual(true, PageWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, PageWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("px", PageWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("px", PageWithGuard.OnNavigatedToParameter);
            Assert.AreEqual(true, PageWithGuard.OnNavigatingFromCalled);


            PageWithGuard.Reset();
            PageWithGuard.CanActivate = false;
            await p.PushAsync(typeof(PageA));

            await p.PopAsync("p2");

            // Can activate
            Assert.AreEqual(true, PageWithGuard.CanActivateCalled);
            Assert.AreEqual(false, PageWithGuard.CanActivate);
            Assert.AreEqual("p2", PageWithGuard.CanActivateParameter);

            Assert.AreEqual(false, PageWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, PageWithGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, PageWithGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, PageWithGuard.OnNavigatedToParameter);

            // can deactivate

            PageWithGuard.Reset();
            PageWithGuard.CanDeactivate = false;

            await p.PushAsync(typeof(PageA));

            await p.PushAsync(typeof(PageWithGuard), "p");

            await p.PopAsync("p3");

            Assert.AreEqual(true, PageWithGuard.CanDeactivateCalled);
            Assert.AreEqual(false, PageWithGuard.CanDeactivate);
            Assert.AreEqual(false, PageWithGuard.OnNavigatingFromCalled);

            PageWithGuard.Reset();

            await p.PopAsync("p3");
            Assert.AreEqual(true, PageWithGuard.CanDeactivateCalled);
            Assert.AreEqual(true, PageWithGuard.CanDeactivate);
            Assert.AreEqual(true, PageWithGuard.OnNavigatingFromCalled);
        }

        [TestMethod]
        public async Task Pop_With_Guard_On_Page_And_ViewModel()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushAsync(typeof(PageWithGuardAndViewModelGuard), "p");

            await p.PushAsync(typeof(PageA));

            await p.PopAsync("px");

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("px", PageWithGuardAndViewModelGuard.CanActivateParameter);

            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("px", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatedToCalled);
            Assert.AreEqual("px", PageWithGuardAndViewModelGuard.OnNavigatingToParameter);
            Assert.AreEqual("px", PageWithGuardAndViewModelGuard.OnNavigatedToParameter);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("px", MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("px", MyViewModelWithGuard.OnNavigatedToParameter);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingFromCalled);


            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();
            PageWithGuardAndViewModelGuard.CanActivate = false;

            // page block

            await p.PushAsync(typeof(PageA));

            await p.PopAsync("p2");

            // Can activate
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("p2", PageWithGuardAndViewModelGuard.CanActivateParameter);

            Assert.AreEqual(false, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual(null, MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, PageWithGuardAndViewModelGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, PageWithGuardAndViewModelGuard.OnNavigatedToParameter);

            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatedToParameter);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();
            MyViewModelWithGuard.CanActivate = false;

            // viewmodel block

            await p.PopAsync("p3");

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("p3", PageWithGuardAndViewModelGuard.CanActivateParameter);

            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p3", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, PageWithGuardAndViewModelGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, PageWithGuardAndViewModelGuard.OnNavigatedToParameter);

            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatedToParameter);

            // can deactivate

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();
            PageWithGuardAndViewModelGuard.CanDeactivate = false;

            await p.PushAsync(typeof(PageA));

            await p.PushAsync(typeof(PageWithGuardAndViewModelGuard), "p");

            await p.PopAsync("p3");

            // page block

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivateCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.CanDeactivate);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(false, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();
            MyViewModelWithGuard.CanDeactivate = false;

            // view model block

            await p.PopAsync("p3");
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivate);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);
        }

        [TestMethod]
        public async Task PopModal_With_Guard_On_Page_And_ViewModel()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushModalAsync(typeof(PageWithGuardAndViewModelGuard), "p");

            await p.PushModalAsync(typeof(PageA));

            await p.PopModalAsync("px");

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("px", PageWithGuardAndViewModelGuard.CanActivateParameter);

            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("px", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatedToCalled);
            Assert.AreEqual("px", PageWithGuardAndViewModelGuard.OnNavigatingToParameter);
            Assert.AreEqual("px", PageWithGuardAndViewModelGuard.OnNavigatedToParameter);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("px", MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("px", MyViewModelWithGuard.OnNavigatedToParameter);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingFromCalled);


            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();
            PageWithGuardAndViewModelGuard.CanActivate = false;

            // page block

            await p.PushModalAsync(typeof(PageA));

            await p.PopModalAsync("p2");

            // Can activate
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("p2", PageWithGuardAndViewModelGuard.CanActivateParameter);

            Assert.AreEqual(false, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual(null, MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, PageWithGuardAndViewModelGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, PageWithGuardAndViewModelGuard.OnNavigatedToParameter);

            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatedToParameter);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();
            MyViewModelWithGuard.CanActivate = false;

            // viewmodel block

            await p.PopModalAsync("p3");

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("p3", PageWithGuardAndViewModelGuard.CanActivateParameter);

            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p3", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, PageWithGuardAndViewModelGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, PageWithGuardAndViewModelGuard.OnNavigatedToParameter);

            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatedToParameter);

            // can deactivate

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();
            PageWithGuardAndViewModelGuard.CanDeactivate = false;

            await p.PushModalAsync(typeof(PageA));

            await p.PushModalAsync(typeof(PageWithGuardAndViewModelGuard), "p");

            await p.PopModalAsync("p3");

            // page block

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivateCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.CanDeactivate);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(false, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();
            MyViewModelWithGuard.CanDeactivate = false;

            // view model block

            await p.PopModalAsync("p3");
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivate);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);
        }

        [TestMethod]
        public async Task PopToRoot_With_Guard_On_Page_And_ViewModel()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();

            // - 0 : HomePage
            // - 1 : PageA
            // - 2 : PageB
            await p.PushAsync(typeof(PageWithGuardAndViewModelGuard), "p");

            await p.PushAsync(typeof(PageA));

            await p.PopToRootAsync("px");

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("px", PageWithGuardAndViewModelGuard.CanActivateParameter);

            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("px", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatedToCalled);
            Assert.AreEqual("px", PageWithGuardAndViewModelGuard.OnNavigatingToParameter);
            Assert.AreEqual("px", PageWithGuardAndViewModelGuard.OnNavigatedToParameter);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual("px", MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual("px", MyViewModelWithGuard.OnNavigatedToParameter);
            Assert.AreEqual(true, MyViewModelWithGuard.OnNavigatingFromCalled);


            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();
            PageWithGuardAndViewModelGuard.CanActivate = false;

            // page block

            await p.PushAsync(typeof(PageA));

            await p.PopToRootAsync("p2");

            // Can activate
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("p2", PageWithGuardAndViewModelGuard.CanActivateParameter);

            Assert.AreEqual(false, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual(null, MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, PageWithGuardAndViewModelGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, PageWithGuardAndViewModelGuard.OnNavigatedToParameter);

            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatedToParameter);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();
            MyViewModelWithGuard.CanActivate = false;

            // viewmodel block

            await p.PopToRootAsync("p3");

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanActivate);
            Assert.AreEqual("p3", PageWithGuardAndViewModelGuard.CanActivateParameter);

            Assert.AreEqual(true, MyViewModelWithGuard.CanActivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.CanActivate);
            Assert.AreEqual("p3", MyViewModelWithGuard.CanActivateParameter);

            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, PageWithGuardAndViewModelGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, PageWithGuardAndViewModelGuard.OnNavigatedToParameter);

            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingToCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatedToCalled);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatingToParameter);
            Assert.AreEqual(null, MyViewModelWithGuard.OnNavigatedToParameter);

            // can deactivate

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();
            PageWithGuardAndViewModelGuard.CanDeactivate = false;

            await p.PushAsync(typeof(PageA));

            await p.PushAsync(typeof(PageWithGuardAndViewModelGuard), "p");

            await p.PopToRootAsync("p3");

            // page block

            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivateCalled);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.CanDeactivate);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(false, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);

            PageWithGuardAndViewModelGuard.Reset();
            MyViewModelWithGuard.Reset();
            MyViewModelWithGuard.CanDeactivate = false;

            // view model block

            await p.PopToRootAsync("p3");
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivateCalled);
            Assert.AreEqual(true, PageWithGuardAndViewModelGuard.CanDeactivate);
            Assert.AreEqual(false, PageWithGuardAndViewModelGuard.OnNavigatingFromCalled);

            Assert.AreEqual(true, MyViewModelWithGuard.CanDeactivateCalled);
            Assert.AreEqual(false, MyViewModelWithGuard.CanDeactivate);
            Assert.AreEqual(false, MyViewModelWithGuard.OnNavigatingFromCalled);
        }

        [TestMethod]
        public async Task Events_With_Push()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            bool a = false, b = false, c = false;

            p.Navigating += (sx, e) =>
            {
                a = true;
            };

            p.Navigated += (sx, e) =>
            {
                b = true;
            };

            p.NavigationFailed += (sx, e) =>
            {
                c = true;
            };


            await p.PushAsync(typeof(PageA));

            Assert.AreEqual(false, a);
            Assert.AreEqual(true, b);
            Assert.AreEqual(false, c);

            await p.PushAsync(typeof(PageB));

            Assert.AreEqual(true, a);

            PageWithGuard.Reset();
            PageWithGuard.CanActivate = false;

            await p.PushAsync(typeof(PageWithGuard));

            Assert.AreEqual(true, c);

            p = null;
        }

        [TestMethod]
        public async Task Events_With_PushModal()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            bool a = false, b = false, c = false;

            p.Navigating += (sx, e) =>
            {
                a = true;
            };

            p.Navigated += (sx, e) =>
            {
                b = true;
            };

            p.NavigationFailed += (sx, e) =>
            {
                c = true;
            };


            await p.PushModalAsync(typeof(PageA));

            Assert.AreEqual(false, a);
            Assert.AreEqual(true, b);
            Assert.AreEqual(false, c);

            await p.PushModalAsync(typeof(PageB));

            Assert.AreEqual(true, a);

            PageWithGuard.Reset();
            PageWithGuard.CanActivate = false;

            await p.PushModalAsync(typeof(PageWithGuard));

            Assert.AreEqual(true, c);

            p = null;
        }

        [TestMethod]
        public async Task Events_With_Pop()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            bool a = false, b = false, c = false;

            p.Navigating += (sx, e) =>
            {
                a = true;
            };

            p.Navigated += (sx, e) =>
            {
                b = true;
            };

            p.NavigationFailed += (sx, e) =>
            {
                c = true;
            };

            await p.PushAsync(typeof(HomePage));
            await p.PushAsync(typeof(PageA));

            a = false;
            b = false;
            c = false;

            await p.PopAsync();

            Assert.AreEqual(true, a);
            Assert.AreEqual(true, b);
            Assert.AreEqual(false, c);

            a = false;
            b = false;
            c = false;

            PageWithGuard.Reset();

            await p.PushAsync(typeof(PageA));
            await p.PushAsync(typeof(PageWithGuard));

            PageWithGuard.CanDeactivate = false;

            await p.PopAsync();

            Assert.AreEqual(true, c);

            p = null;
        }

        [TestMethod]
        public async Task Events_With_PopModal()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            bool a = false, b = false, c = false;

            p.Navigating += (sx, e) =>
            {
                a = true;
            };

            p.Navigated += (sx, e) =>
            {
                b = true;
            };

            p.NavigationFailed += (sx, e) =>
            {
                c = true;
            };

            await p.PushModalAsync(typeof(HomePage));
            await p.PushModalAsync(typeof(PageA));

            a = false;
            b = false;
            c = false;

            await p.PopModalAsync();

            Assert.AreEqual(true, a);
            Assert.AreEqual(true, b);
            Assert.AreEqual(false, c);

            a = false;
            b = false;
            c = false;

            PageWithGuard.Reset();

            await p.PushModalAsync(typeof(PageWithGuard));

            PageWithGuard.CanDeactivate = false;

            await p.PopModalAsync();

            Assert.AreEqual(true, c);

            p = null;
        }

        [TestMethod]
        public async Task Events_With_PopToRoot()
        {
            var s = new FakeNavigationStrategy();
            var p = new PageNavigationService(s);

            bool a = false, b = false, c = false;

            p.Navigating += (sx, e) =>
            {
                a = true;
            };

            p.Navigated += (sx, e) =>
            {
                b = true;
            };

            p.NavigationFailed += (sx, e) =>
            {
                c = true;
            };

            await p.PushAsync(typeof(HomePage));
            await p.PushAsync(typeof(PageA));

            a = false;
            b = false;
            c = false;

            await p.PopToRootAsync();

            Assert.AreEqual(true, a);
            Assert.AreEqual(true, b);
            Assert.AreEqual(false, c);

            a = false;
            b = false;
            c = false;

            PageWithGuard.Reset();

            await p.PushAsync(typeof(PageA));
            await p.PushAsync(typeof(PageWithGuard));

            PageWithGuard.CanDeactivate = false;

            await p.PopToRootAsync();

            Assert.AreEqual(true, c);

            p = null;
        }

        [TestMethod]
        public async Task GoBack_Unhandled()
        {
            var page = new NavigationPage();
            var s = new NavigationPageFacade(page);
            var p = new PageNavigationService(s);

            await p.PushAsync(typeof(HomePage));

            await p.PushAsync(typeof(PageWithViewModel));
            MyViewModel.Reset();
            await page.Navigation.PopAsync();
            Assert.IsTrue(MyViewModel.OnNavigatingFromCalled);

            await p.PushAsync(typeof(PageWithViewModel));
            MyViewModel.Reset();
            p.UnhandleSystemPagePopped();
            await page.Navigation.PopAsync();
            Assert.IsFalse(MyViewModel.OnNavigatingFromCalled);

            await p.PushAsync(typeof(PageWithViewModel));
            p.HandleSystemPagePopped();
            MyViewModel.Reset();
            await page.Navigation.PopAsync();
            Assert.IsTrue(MyViewModel.OnNavigatingFromCalled);
        }
    }

    public class PageWithViewModel : Page
    {
        public PageWithViewModel()
        {
            this.BindingContext = new MyViewModel();
        }
    }

    public class PageWithViewModelAndGuard : Page
    {
        public PageWithViewModelAndGuard()
        {
            this.BindingContext = new MyViewModelWithGuard();
        }
    }

    public class PageWithGuard : Page, INavigatable, IActivatable, IDeactivatable
    {
        public static bool OnNavigatingFromCalled { get; set; }
        public static bool OnNavigatingToCalled { get; set; }
        public static bool OnNavigatedToCalled { get; set; }

        public static bool CanActivate { get; set; } = true;
        public static bool CanDeactivate { get; set; } = true;
        public static bool CanActivateCalled { get; set; }
        public static bool CanDeactivateCalled { get; set; }
        public static object CanActivateParameter { get; set; }

        public static object OnNavigatingToParameter { get; set; }
        public static object OnNavigatedToParameter { get; set; }

        public static void Reset()
        {
            OnNavigatingToCalled = false;
            OnNavigatingToParameter = null;
            OnNavigatedToCalled = false;
            OnNavigatedToParameter = null;
            OnNavigatingFromCalled = false;
            CanActivate = true;
            CanDeactivate = true;
            CanActivateCalled = false;
            CanDeactivateCalled = false;
            CanActivateParameter = null;
        }

        public void OnNavigatingTo(object parameter)
        {
            OnNavigatingToCalled = true;
            OnNavigatingToParameter = parameter;
        }

        public void OnNavigatedTo(object parameter)
        {
            OnNavigatedToCalled = true;
            OnNavigatedToParameter = parameter;
        }

        public void OnNavigatingFrom()
        {
            OnNavigatingFromCalled = true;
        }

        public Task<bool> CanDeactivateAsync()
        {
            CanDeactivateCalled = true;
            return Task.FromResult(CanDeactivate);
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            CanActivateCalled = true;
            CanActivateParameter = parameter;
            return Task.FromResult(CanActivate);
        }
    }

    public class PageWithGuardAndViewModelGuard : Page, INavigatable, IActivatable, IDeactivatable
    {
        public PageWithGuardAndViewModelGuard()
        {
            this.BindingContext = new MyViewModelWithGuard();
        }

        public static bool OnNavigatingFromCalled { get; set; }
        public static bool OnNavigatingToCalled { get; set; }
        public static bool OnNavigatedToCalled { get; set; }

        public static bool CanActivate { get; set; } = true;
        public static bool CanDeactivate { get; set; } = true;
        public static bool CanActivateCalled { get; set; }
        public static bool CanDeactivateCalled { get; set; }
        public static object CanActivateParameter { get; set; }

        public static object OnNavigatingToParameter { get; set; }
        public static object OnNavigatedToParameter { get; set; }

        public static void Reset()
        {
            OnNavigatingToCalled = false;
            OnNavigatingToParameter = null;
            OnNavigatedToCalled = false;
            OnNavigatedToParameter = null;
            OnNavigatingFromCalled = false;
            CanActivate = true;
            CanDeactivate = true;
            CanActivateCalled = false;
            CanDeactivateCalled = false;
            CanActivateParameter = null;
        }

        public void OnNavigatingTo(object parameter)
        {
            OnNavigatingToCalled = true;
            OnNavigatingToParameter = parameter;
        }

        public void OnNavigatedTo(object parameter)
        {
            OnNavigatedToCalled = true;
            OnNavigatedToParameter = parameter;
        }

        public void OnNavigatingFrom()
        {
            OnNavigatingFromCalled = true;
        }

        public Task<bool> CanDeactivateAsync()
        {
            CanDeactivateCalled = true;
            return Task.FromResult(CanDeactivate);
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            CanActivateCalled = true;
            CanActivateParameter = parameter;
            return Task.FromResult(CanActivate);
        }
    }

    public class MyViewModel : INavigatable
    {
        public static bool OnNavigatingFromCalled { get; set; }
        public static bool OnNavigatingToCalled { get; set; }
        public static bool OnNavigatedToCalled { get; set; }

        public static object OnNavigatingToParameter { get; set; }
        public static object OnNavigatedToParameter { get; set; }

        public static void Reset()
        {
            OnNavigatingToCalled = false;
            OnNavigatingToParameter = null;
            OnNavigatedToCalled = false;
            OnNavigatedToParameter = null;
            OnNavigatingFromCalled = false;
        }

        public void OnNavigatingTo(object parameter)
        {
            OnNavigatingToCalled = true;
            OnNavigatingToParameter = parameter;
        }

        public void OnNavigatedTo(object parameter)
        {
            OnNavigatedToCalled = true;
            OnNavigatedToParameter = parameter;
        }

        public void OnNavigatingFrom()
        {
            OnNavigatingFromCalled = true;

        }


    }

    public class MyViewModelWithGuard : INavigatable, IActivatable, IDeactivatable
    {
        public static bool OnNavigatingFromCalled { get; set; }
        public static bool OnNavigatingToCalled { get; set; }
        public static bool OnNavigatedToCalled { get; set; }

        public static bool CanActivate { get; set; } = true;
        public static bool CanDeactivate { get; set; } = true;
        public static bool CanActivateCalled { get; set; }
        public static bool CanDeactivateCalled { get; set; }
        public static object CanActivateParameter { get; set; }

        public static object OnNavigatingToParameter { get; set; }
        public static object OnNavigatedToParameter { get; set; }

        public static void Reset()
        {
            OnNavigatingToCalled = false;
            OnNavigatingToParameter = null;
            OnNavigatedToCalled = false;
            OnNavigatedToParameter = null;
            OnNavigatingFromCalled = false;
            CanActivate = true;
            CanDeactivate = true;
            CanActivateCalled = false;
            CanDeactivateCalled = false;
            CanActivateParameter = null;
        }

        public void OnNavigatingTo(object parameter)
        {
            OnNavigatingToCalled = true;
            OnNavigatingToParameter = parameter;
        }

        public void OnNavigatedTo(object parameter)
        {
            OnNavigatedToCalled = true;
            OnNavigatedToParameter = parameter;
        }

        public void OnNavigatingFrom()
        {
            OnNavigatingFromCalled = true;
        }

        public Task<bool> CanDeactivateAsync()
        {
            CanDeactivateCalled = true;
            return Task.FromResult(CanDeactivate);
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            CanActivateCalled = true;
            CanActivateParameter = parameter;
            return Task.FromResult(CanActivate);
        }
    }
}
