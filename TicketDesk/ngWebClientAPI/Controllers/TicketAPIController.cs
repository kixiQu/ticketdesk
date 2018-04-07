﻿using System;
using System.Threading.Tasks;
using System.Web.Http;
using TicketDesk.Domain;
using TicketDesk.Domain.Model;
using System.Web.Mvc;
using System.Linq;
using System.Net;
using ngWebClientAPI.Models;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace ngWebClientAPI.Controllers
{
    [System.Web.Http.RoutePrefix("api/ticket")]
    public class TicketAPIController : ApiController
    {
        private TicketController ticketController = new TicketController(new TdDomainContext());

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("")]
        public async Task<JObject> getAllTickets()
        {
            try
            {
                var model = await ticketController.GetTicketList(); //returns list of all tickets
                List<FrontEndTicket> TicketList = new List<FrontEndTicket>();
                foreach (var item in model)
                {
                    TicketList.Add(APITicketConversion.ConvertGETTicket(item));
                }
                JList lst = new JList();
                lst.list = TicketList;
                return JObject.FromObject(lst);
            }
            catch (Exception ex)
            {
                return JObject.FromObject(ex.Message);
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("{ticketId}")]
        public async Task<JObject> getSingleTicket(Int64 ticketId)
        {
            int convertedId = APITicketConversion.ConvertTicketId(ticketId);//for when we get semantic numbering to front end
            Ticket model = await ticketController.getTicket(convertedId); //Expect full semantic id
            if (model == null)
            {
                return null; //Should probably error handle better here... Leaving as null for now
            }
            try
            {
                FrontEndTicket retVal = APITicketConversion.ConvertGETTicket(model);
                return JObject.FromObject(retVal);
            }
            catch
            {
                return null;
            }

        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("")]
        public async Task<HttpStatusCodeResult> createTicket([FromBody]JObject jsonData)
        {
            HttpStatusCodeResult result; 
            //convert data to comment and ID
            try
            {
                Ticket ticket = APITicketConversion.ConvertPOSTTicket(jsonData);
                bool status = await ticketController.CreateTicketAsync(ticket);
               result = new HttpStatusCodeResult(HttpStatusCode.OK, APITicketConversion.ConvertTicketId(ticket.TicketId).ToString());
                
            }
            catch (Exception ex)
            {
                result = new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.ToString());
            }
            return result;
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("events/{ticketId}")]
        public async Task<JObject> GetEvents(Int64 ticketId)
        {
            int convertedId = APITicketConversion.ConvertTicketId(ticketId);//for when we get semantic numbering to front end
            Ticket model = await ticketController.getTicket(convertedId);
            if (model == null)
            {
                return null; // Should probably handle errors better here. Returning Null for now
            }
            try
            {
                EventList eventList = new EventList();
                //eventList.list = model.TicketEvents.ToList();
                List<TicketEvent> events = model.TicketEvents.ToList();
                eventList.list = new List<FrontEndEvent>();
                foreach(var item in events)
                {
                    eventList.list.Add(APITicketConversion.ConvertEvent(item));
                }
                return JObject.FromObject(eventList);
            }
            catch(Exception ex)
            {
                return JObject.FromObject(ex);
            }

        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("categories")]
        public async Task<JObject> getCategories([FromBody]JObject jsonData)
        {
            try
            {
                Dictionary<string, List<string>> dict = GlobalConfig.categories;
                return JObject.FromObject(dict);
            }
            catch (Exception ex)
            {
                return JObject.FromObject(ex);
            }
        }
    }
}