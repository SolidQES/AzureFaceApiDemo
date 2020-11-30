using CognitiveServicesPOC.Resources;
using CognitiveServicesPOC.Services;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveServicesPOC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("· Ejemplo de reconocimiento facial" + Environment.NewLine);
            await RecognizeFaceTest();

            Console.WriteLine(Environment.NewLine + "· Ejemplo de similitudes faciales" + Environment.NewLine);
            await SimilarFaceTest();
        }

        private static async Task SimilarFaceTest()
        {
            SimilarityService similarityService = new SimilarityService(StaticConfiguration.FaceApiUrl, StaticConfiguration.FaceApiKey, "test1", "Tony");

            // Añadimos las imágenes de 'Tony' a la lista ya creada
            await similarityService.TrainService(Images.GetTrainFacesImages(Images.TestPerson.Tony));

            Console.WriteLine("Comparando una imagen de 'Tony' con las ya registradas");
            var similarityDetected = await similarityService.GetSimilarities(Images.GetTestFaceImage(Images.TestPerson.Tony));

            similarityDetected.OrderByDescending(_ => _.Confidence)
                .ToList().ForEach(_ => Console.WriteLine($"Confianza: {_.Confidence} \t| Id imagen: {_.PersistedFaceId}"));

            Console.WriteLine("Comparando una imagen de 'Peter' con las registradas de 'Tony'");
            similarityDetected = await similarityService.GetSimilarities(Images.GetTestFaceImage(Images.TestPerson.Peter));

            similarityDetected.OrderByDescending(_ => _.Confidence)
                .ToList().ForEach(_ => Console.WriteLine($"Confianza: {_.Confidence} \t| Id imagen: {_.PersistedFaceId}"));

            Console.WriteLine($"Comparando una imagen de 'Steve' con las registradas de 'Tony'");
            similarityDetected = await similarityService.GetSimilarities(Images.GetTestFaceImage(Images.TestPerson.Steve));

            similarityDetected.OrderByDescending(_ => _.Confidence)
                .ToList().ForEach(_ => Console.WriteLine($"Confianza: {_.Confidence} \t| Id imagen: {_.PersistedFaceId}"));

            // Eliminamos la lista de imágenes de prueba
            await similarityService.DeletePersonFaceList();
        }

        private static async Task RecognizeFaceTest()
        {
            RecognitionService recognitionService = new RecognitionService(StaticConfiguration.FaceApiUrl, StaticConfiguration.FaceApiKey, "test1");

            Console.WriteLine("Entrenando imágenes de 'Tony'");
            await recognitionService.TrainService(Images.GetTrainFacesImages(Images.TestPerson.Tony), "Tony");

            Console.WriteLine("Entrenando imágenes de 'Peter'");
            await recognitionService.TrainService(Images.GetTrainFacesImages(Images.TestPerson.Peter), "Peter");

            Console.WriteLine("Identificando nueva imagen de 'Tony'");
            var recognizedPeople = await recognitionService.RecognizeInGroup(Images.GetTestFaceImage(Images.TestPerson.Tony));

            Console.WriteLine($"Persona reconocida: {(!string.IsNullOrEmpty(recognizedPeople) ? recognizedPeople : "?")}" + Environment.NewLine 
                + "Identificando nueva imagen de 'Peter'");

            recognizedPeople = await recognitionService.RecognizeInGroup(Images.GetTestFaceImage(Images.TestPerson.Peter));

            Console.WriteLine($"Persona reconocida: {(!string.IsNullOrEmpty(recognizedPeople) ? recognizedPeople : "?")}" + Environment.NewLine 
                + "Identificando nueva imagen de 'Steve'");

            recognizedPeople = await recognitionService.RecognizeInGroup(Images.GetTestFaceImage(Images.TestPerson.Steve));

            Console.WriteLine($"Persona reconocida: {(!string.IsNullOrEmpty(recognizedPeople) ? recognizedPeople : "?")}");

            await recognitionService.CleanGroup();
        }
    }
}
