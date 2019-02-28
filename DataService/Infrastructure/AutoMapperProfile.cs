using AutoMapper;
using DataService.Models;
using DataService.Repositories;
using DataService.Services;
using DataService.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataService.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Barbers, BarberViewModel>();
        }
    }
}
