using DocumentFormat.OpenXml.Packaging;
using System.IO;
using OpenXmlPowerTools;
using DocumentFormat.OpenXml.Wordprocessing;
//using Microsoft.Office.Interop.Word;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;




namespace XML_Handler
{
    class DocBuilder
    {
        static public void CreateLetterToDisabled(string savePath, XmlPerson person, InvalidLetterData letterData)
        {
            string templateFilePath = "Templates\\Письмо гражданину (шаблон).docx";
            string docName = $"{person.fullName!.surname} {person.fullName.name} {person.fullName.patronymic}";
            string outputFilePath = Path.Combine(savePath, $"{docName}.docx");

            Dictionary<string, string> dataDictionary = new()
                        {
                            { "{name}",          person.fullName!.name },
                            { "{surname}",       person.fullName.surname },
                            { "{patronymic}",    person.fullName.patronymic },
                            { "{address}",       person.address! },
                            { "{department}",    person.department! },
                            { "{signatory}",     letterData.signatory.Name! },
                            { "{signatoryPost}", letterData.signatory.Post! },
                            { "{executor}",      letterData.executor.Name! },
                            { "{executorPhone}", letterData.executor.Phone! }
                        };

            // копируем файл в указанное место
            File.Copy(templateFilePath, outputFilePath, true);

            // редактируем файл
            foreach (var data in dataDictionary)
            {
                using WordprocessingDocument doc = WordprocessingDocument.Open(outputFilePath, true);

                // заменяем плейсхолдеры на данные
                TextReplacer.SearchAndReplace(wordDoc: doc, search: data.Key, replace: data.Value, matchCase: false);

                doc.Save();
            }

            // вырезано за ненадобностью

            //// конвертация в PDF
            //var appWord = new Application();
            //if (appWord.Documents != null)
            //{
            //    var wordDocument = appWord.Documents.Open(outputFilePath);
            //    string pdfDocName = Path.Combine(savePath, $"{docName}.pdf");
            //    if (wordDocument != null)
            //    {
            //        wordDocument.ExportAsFixedFormat(pdfDocName,
            //        WdExportFormat.wdExportFormatPDF);
            //        wordDocument.Close();
            //    }
            //    appWord.Quit();
            //}

        }



        static public void CreateLetterToHead(string savePath, HeadLetterData letterData)
        {
            string templateFilePath = "Templates\\Письмо главе (шаблон).docx";
            string docName = $"{letterData.district.Name}";
            string outputFilePath = Path.Combine(savePath, $"{docName}.docx");

            Dictionary<string, string> dataDictionary = new()
                        {
                            { "{districtGC}",    letterData.district.DistrictGc! },
                            { "{greetings}",    (letterData.district.Gender == "М") ? "Уважаемый" : "Уважаемая" },
                            { "{headDC}",        letterData.district.HeadDc! },
                            { "{head}",          letterData.district.Head! },
                            { "{department}",    letterData.district.Department! },
                            { "{signatory}",     letterData.signatory.Name! },
                            { "{signatoryPost}", letterData.signatory.Post! },
                            { "{executor}",      letterData.executor.Name! },
                            { "{executorPhone}", letterData.executor.Phone! }
                        };

            // копируем файл в указанное место
            File.Copy(templateFilePath, outputFilePath, true);

            // редактируем файл
            foreach (var data in dataDictionary)
            {
                using WordprocessingDocument doc = WordprocessingDocument.Open(outputFilePath, true);

                // заменяем плейсхолдеры на данные
                TextReplacer.SearchAndReplace(wordDoc: doc, search: data.Key, replace: data.Value, matchCase: false);

                doc.Save();
            }

            // создаём список инвалидов в приложении к письму
            using (WordprocessingDocument doc = WordprocessingDocument.Open(outputFilePath, true))
            {
                Table table = doc.MainDocumentPart!.Document.Body!.Descendants<Table>().ElementAt(2);

                TableRow row = table.Elements<TableRow>().ElementAt(0);
                TableCell cell = row.Elements<TableCell>().ElementAt(0);

                Paragraph p = cell.Elements<Paragraph>().First();
                Run  r = p.Elements<Run>().First();
                Text t = r.Elements<Text>().First();
                t.Text = "1. ";

                var fullName = CreateTableCell(letterData.personList[0].Name!);
                var birthDate = CreateTableCell(letterData.personList[0].BirthDate!);

                row.Append(fullName);
                row.Append(birthDate);

                for (int i = 1; i< letterData.personList.Count; i++)
                {
                    var invalidRow = new TableRow();

                    var numberCell    = CreateTableCell((i+1).ToString() + ". ");
                    var fullNameCell  = CreateTableCell(letterData.personList[i].Name!);
                    var birthDateCell = CreateTableCell(letterData.personList[i].BirthDate!);

                    invalidRow.Append(numberCell);
                    invalidRow.Append(fullNameCell);
                    invalidRow.Append(birthDateCell);

                    table.Append(invalidRow);
                }


                doc.Save();
            }

            // вырезано за ненадобностью

            //// конвертация в PDF
            //var appWord = new Application();
            //if (appWord.Documents != null)
            //{
            //    //yourDoc is your word document
            //    var wordDocument = appWord.Documents.Open(outputFilePath);
            //    string pdfDocName = Path.Combine(savePath, $"{docName}.pdf");
            //    if (wordDocument != null)
            //    {
            //        wordDocument.ExportAsFixedFormat(pdfDocName,
            //        WdExportFormat.wdExportFormatPDF);
            //        wordDocument.Close();
            //    }
            //    appWord.Quit();
            //}
        }

        private static TableCell CreateTableCell(string cellText)
        {
            var cell = new TableCell();

            var paragraph = new Paragraph();
            var run = new Run();
            var text = new Text(cellText);

            RunProperties runProperties = new();
            //FontSize fontSize1 = new() {  };
            runProperties.Append(new FontSize() { Val = "26" });

            run.Append(runProperties);
            run.Append(text);

            paragraph.Append(run);
            cell.Append(paragraph);

            return cell;
        }
    }
}