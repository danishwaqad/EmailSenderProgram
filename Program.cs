using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace EmailSenderProgram
{
    internal class EmailSenderProgram
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Sending Welcome mail");
            bool successWelcome = SendWelcomeEmail();

            bool successComeback = false;
#if DEBUG
            // Debug mode, always send Comeback mail
            Console.WriteLine("Send Comeback mail");
            successComeback = SendComebackEmail("EOComebackToUs");
#else
            // Every Sunday run Comeback mail
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                Console.WriteLine("Send Comeback mail");
                successComeback = SendComebackEmail("EOComebackToUs");
            }
#endif

            // Check if the sending went OK
            if (successWelcome && successComeback)
            {
                Console.WriteLine("All mails are sent, I hope...");
            }
            else
            {
                Console.WriteLine("Oops, something went wrong when sending mail (I think...)");
            }

            Console.ReadKey();
        }

        private static bool SendWelcomeEmail()
        {
            try
            {
                List<Customer> customers = DataLayer.ListCustomers();

                foreach (Customer customer in customers)
                {
                    if (customer.CreatedDateTime > DateTime.Now.AddDays(-1))
                    {
                        MailMessage message = CreateWelcomeEmail(customer.Email);
                        SendMessage(message);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending Welcome mail: {ex.Message}");
                return false;
            }
        }

        private static bool SendComebackEmail(string voucher)
        {
            try
            {
                List<Customer> customers = DataLayer.ListCustomers();
                List<Order> orders = DataLayer.ListOrders();

                foreach (Customer customer in customers)
                {
                    if (IsEligibleForComebackEmail(customer, orders))
                    {
                        MailMessage message = CreateComebackEmail(customer.Email, voucher);
                        SendMessage(message);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending Comeback mail: {ex.Message}");
                return false;
            }
        }

        private static bool IsEligibleForComebackEmail(Customer customer, List<Order> orders)
        {
            foreach (Order order in orders)
            {
                if (customer.Email == order.CustomerEmail)
                {
                    return false;
                }
            }
            return true;
        }

        private static MailMessage CreateWelcomeEmail(string email)
        {
            MailMessage message = new MailMessage();
            message.To.Add(email);
            message.Subject = "Welcome as a new customer at EO!";
            message.From = new MailAddress("danishwaqad2@gmail.com");
            message.Body = $"Hi {email},<br>We would like to welcome you as a customer on our site!<br><br>Best Regards,<br>EO Team";
            return message;
        }

        private static MailMessage CreateComebackEmail(string email, string voucher)
        {
            MailMessage message = new MailMessage();
            message.To.Add(email);
            message.Subject = "We miss you as a customer";
            message.From = new MailAddress("danishwaqad2@gmail.com");
            message.Body = $"Hi {email},<br>We miss you as a customer. Our shop is filled with nice products. Here is a voucher that gives you 50 kr to shop for.<br>Voucher: {voucher}<br><br>Best Regards,<br>EO Team";
            return message;
        }

        private static void SendMessage(MailMessage message)
        {
#if DEBUG
            // Don't send mails in debug mode, just write the emails in console
            Console.WriteLine($"Send mail to: {message.To[0]}");
#else
            // Create a SmtpClient to Gmail SMTP server
            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))
            {
                smtp.Port = 587; // Set the port number for Gmail
                smtp.EnableSsl = true; // Enable SSL/TLS
                smtp.UseDefaultCredentials = false; // Do not use default credentials

                // Provide your Gmail credentials here
                smtp.Credentials = new System.Net.NetworkCredential("danishwaqad2@gmail.com", "yobq hdpf lxgj xqkm");

                // Send mail
                smtp.Send(message);
            }
#endif
        }
    }
}

public class Customer
{
    public string Email { get; set; }
    public DateTime CreatedDateTime { get; set; }
}

public class Order
{
    public string CustomerEmail { get; set; }
    public DateTime OrderDatetime { get; set; }
}

class DataLayer
{
    public static List<Customer> ListCustomers()
    {
        return new List<Customer>
        {
            new Customer { Email = "mail1@mail.com", CreatedDateTime = DateTime.Now.AddHours(-7) },
            new Customer { Email = "mail2@mail.com", CreatedDateTime = DateTime.Now.AddDays(-1) },
            new Customer { Email = "mail3@mail.com", CreatedDateTime = DateTime.Now.AddMonths(-6) },
            new Customer { Email = "mail4@mail.com", CreatedDateTime = DateTime.Now.AddMonths(-1) },
            new Customer { Email = "mail5@mail.com", CreatedDateTime = DateTime.Now.AddMonths(-2) },
            new Customer { Email = "mail6@mail.com", CreatedDateTime = DateTime.Now.AddDays(-5) }
        };
    }

    public static List<Order> ListOrders()
    {
        return new List<Order>
        {
            new Order { CustomerEmail = "mail3@mail.com", OrderDatetime = DateTime.Now.AddMonths(-6) },
            new Order { CustomerEmail = "mail5@mail.com", OrderDatetime = DateTime.Now.AddMonths(-2) },
            new Order { CustomerEmail = "mail6@mail.com", OrderDatetime = DateTime.Now.AddDays(-2) }
        };
    }
}
