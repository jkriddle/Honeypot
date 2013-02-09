using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Honeypot.Domain.Filters;
using Honeypot.Web.Areas.Api.Models;

namespace Honeypot.Web.Bootstrap
{
    public static class AutoMapperServiceBootstrap
    {
        public static void Init()
        {
            // Initialize mapping. Do not copy null values over defaults.
            Mapper.CreateMap<UserListInputModel, UserFilter>().ForAllMembers(opt => opt.Condition(srs => !srs.IsSourceValueNull));
            Mapper.CreateMap<LogListInputModel, LogFilter>().ForAllMembers(opt => opt.Condition(srs => !srs.IsSourceValueNull));
            Mapper.CreateMap<ResourceListInputModel, ResourceFilter>().ForAllMembers(opt => opt.Condition(srs => !srs.IsSourceValueNull));
        }
    }
}