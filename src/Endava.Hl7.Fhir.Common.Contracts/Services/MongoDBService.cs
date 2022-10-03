using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;
using Endava.Hl7.Fhir.Common.Contracts.Dto;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace Endava.Hl7.Fhir.Common.Contracts.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Patient> _patientCollection;
        private readonly FhirClient _fhirClient;

        public MongoDBService(IMongoCollection<Patient> patientCollection, FhirClient fhirClient)
        {
            _patientCollection = patientCollection;
            _fhirClient = fhirClient;
        }

        public MongoDBService(IOptions<Models.MongoDBSettings> mongoDBSettings)
        {
            var settings = mongoDBSettings.Value;
            var client = new MongoClient("mongodb+srv://fatihcolak42:burdur15@cluster0.3qinvcw.mongodb.net/test");
            var database = client.GetDatabase(settings.DatabaseName);
            _patientCollection = database.GetCollection<Patient>(settings.CollectionName);
        }

        public async Task<IEnumerable<BsonDocument>> GetAllAsync()
        {
            var projection = Builders<Patient>.Projection.Exclude("_id");
            var list =  await _patientCollection.Find(x => true).Project(projection).ToListAsync();
            return list;
        }
            
        

        public async Task<Patient> GetAsync(string id) =>
            await _patientCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async System.Threading.Tasks.Task CreateAsync(Patient patient) =>
            await _patientCollection.InsertOneAsync(patient);

        public async System.Threading.Tasks.Task DeleteAsync(string id) =>
            await _patientCollection.DeleteOneAsync(x => x.Id == id);
    }
}
