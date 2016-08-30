﻿using ChatMe.DataAccess.Entities;
using ChatMe.DataAccess.Interfaces;
using ChatMe.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Mvc;
using ChatMe.BussinessLogic;
using ChatMe.BussinessLogic.Services.Abstract;
using AutoMapper;
using ChatMe.BussinessLogic.DTO;
using System.Threading.Tasks;

namespace ChatMe.Web.Controllers
{
    [RoutePrefix("api/posts")]
    public class PostController : Controller
    {
        private IPostService postService;

        public PostController(IPostService postService) {
            this.postService = postService;
        }

        [HttpGet]
        [Route("{userId}")]
        public ActionResult GetAll(string userId, int startIndex = 0, int count = 0) {
            var postsData = postService.GetChunk(userId, startIndex, count);
            Mapper.Initialize(cfg => cfg.CreateMap<PostDTO, PostViewModel>()
                .ForMember("AvatarUrl", opt => opt.MapFrom(p =>
                    Url.Action("GetAvatar", "User", new { id = p.AuthorId }))
                )
                .ForMember("AuthorLink", opt => opt.MapFrom(p =>
                    Url.RouteUrl("UserProfile", new { id = p.AuthorId }))
                )
            );

            var posts = Mapper.Map<IEnumerable<PostViewModel>>(postsData);

            return Json(posts.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("{userId}/{postId}")]
        public PostViewModel Get(string userId, int postId) {
            var postData = postService.Get(userId, postId);

            Mapper.Initialize(cfg => cfg.CreateMap<PostDTO, PostViewModel>()
                .ForMember("AvatarUrl", opt => opt.MapFrom(p =>
                    Url.Action("GetAvatar", "User", new { id = p.AuthorId }))
                )
                .ForMember("AuthorLink", opt => opt.MapFrom(p =>
                    Url.RouteUrl("UserProfile", new { id = p.AuthorId }))
                )
            );

            return Mapper.Map<PostViewModel>(postData);
        }

        [HttpPost]
        [Route("{userId}")]
        public async Task Post(string userId, NewPostViewModel postModel) {
            var newPostData = new NewPostDTO {
                Body = postModel.Body,
                UserId = userId
            };

            await postService.Create(newPostData);
        }

        [HttpPut]
        [Route("{postId}")]
        public async Task Put(int postId, NewPostViewModel postModel) {
            var newPostData = new NewPostDTO {
                Body = postModel.Body
            };

            await postService.Update(newPostData, postId);
        }

        [HttpDelete]
        [Route("{postId}")]
        public async Task Delete(int postId) {
            await postService.Delete(postId);
        }
    }
}
