﻿using System.Diagnostics;
using BankSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Models.BankAccount;
    using Models.MoneyTransfer;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Services.Models.MoneyTransfer;

    public class HomeController : Controller
    {
        private readonly IBankAccountService bankAccountService;
        private readonly IUserService userService;
        private readonly IMoneyTransferService moneyTransferService;

        public HomeController(
            IBankAccountService bankAccountService,
            IUserService userService,
            IMoneyTransferService moneyTransferService)
        {
            this.bankAccountService = bankAccountService;
            this.userService = userService;
            this.moneyTransferService = moneyTransferService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = await this.userService.GetUserIdAsyncByUsername(this.User.Identity.Name);
            var bankAccounts = (await this.bankAccountService.GetAllUserAccountsAsync<BankAccountIndexServiceModel>(userId))
                .Select(Mapper.Map<BankAccountIndexViewModel>)
                .ToList();
            var moneyTransfers = (await this.moneyTransferService
                    .GetAllMoneyTransfersForGivenUserByUserIdAsync<MoneyTransferListingServiceModel>(userId))
                .Select(Mapper.Map<MoneyTransferListingViewModel>)
                .ToList();

            var viewModel = new HomeViewModel
            {
                UserBankAccounts = bankAccounts,
                MoneyTransfers = moneyTransfers,
            };
            return this.View(viewModel);
        }
        
        public IActionResult IndexGuest()
        {
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
