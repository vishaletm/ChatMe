﻿using ChatMe.BussinessLogic.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatMe.BussinessLogic.DTO;
using ChatMe.DataAccess.Entities;
using Microsoft.AspNet.Identity;
using ChatMe.DataAccess.Interfaces;

namespace ChatMe.BussinessLogic.Services
{
    public class DialogService : IDialogService
    {
        private IUnitOfWork unitOfWork;
        private IUserService userService;

        public DialogService(IUnitOfWork unitOfWork, IUserService userService) {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
        }

        public async Task<bool> Create(NewDialogDTO data) {
            var users = data.UserIds
                .Select(id => unitOfWork.Users.Get(id))
                .ToList();

            var newDialog = new Dialog {
                Users = users,
                CreateTime = DateTime.Now
            };

            unitOfWork.Dialogs.Create(newDialog);
            await unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> Delete(int dialogId) {
            unitOfWork.Dialogs.Delete(dialogId);
            await unitOfWork.SaveAsync();
            return true;
        }

        public IEnumerable<DialogPreviewDTO> GetChunk(AppUserManager userManager, string userId, int startIndex, int chunkSize) {
            var me = userManager.FindById(userId);
            var dialogs = me.Dialogs
                .OrderByDescending(d => (d.LastMessageTime.HasValue ? d.LastMessageTime : d.CreateTime))
                .Skip(startIndex)
                .Select(d => new DialogPreviewDTO {
                    Id = d.Id,
                    LastMessage = d.Messages
                        .OrderByDescending(m => m.Time)
                        .FirstOrDefault()
                        .Body,
                    LastMessageAuthor = userService.GetUserDisplayName(d.Messages
                        .OrderByDescending(m => m.Time)
                        .FirstOrDefault()
                        .User),
                    Users = d.Users
                        .Where(u => u.Id != userId)
                        .Select(u => new UserInfoDTO {
                            Id = u.Id,
                            FirstName = u.UserInfo.FirstName,
                            LastName = u.UserInfo.LastName,
                            UserName = u.UserName,
                            AvatarFilename = u.UserInfo.AvatarFilename
                        })
                });

            if (chunkSize != 0) {
                dialogs = dialogs.Take(chunkSize);
            }

            return dialogs;
        }
    }
}
