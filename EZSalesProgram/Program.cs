using EZSalesProgram.PaymentTypes;
using EZSalesProgram.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace EZSalesProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Universal Variables
            string productFilePath = "../../../Processing/txtFiles/Products.txt";
            List<Product> productList = new List<Product>(); // Makes Product List
            List<Product> cartList = new List<Product>(); // Makes Cart List
            List<string> stringList = new List<string>();

            #endregion

            PrintHeader(); // ----------- prints header display
            BuildProductList(productFilePath, productList, stringList); // ------ read Inventory from product.txt file
            PrintInventory(productList); // ------- prints product list
            AddItemToCart(productList, cartList); // ------ adds user selection to cart & displays current cart items w/price
            CalculateCartTotal(cartList); // -------- prints Cart list for checkout
           

            Console.WriteLine("\n***Thank you for your purchase! Please come shop with us again***!\n");
            Environment.Exit(0);


        }
        private static void BuildProductList(string productFilePath, List<Product> productList, List<string> stringList)
        {
            StreamReader productReader = new StreamReader($@"{productFilePath}");
            string fileData = "";
            string nextLine = productReader.ReadLine();

            while (nextLine != null)
            {
                fileData += nextLine + "\n";
                stringList.Add(nextLine);
                nextLine = productReader.ReadLine();
            }

            foreach (string product in stringList)
            {
                string[] info = product.Split(',');

                Product temp = new Product(info[0], info[1], info[2], double.Parse(info[3]), Int32.Parse(info[4]));
                productList.Add(temp);
            }

            productReader.Close();
        }

        private static void CalculateCartTotal(List<Product> cartList)
        {
            Console.WriteLine("\nCurrently you have these items in your cart: ");
            Console.WriteLine("\n\t***Your Shopping Cart***\n-------------------------------------------------");

            double itemSalesTax = 0;
            double cartTotalSalesTax = 0;
            double cartSubTotal = 0;

            foreach (Product cartItem in cartList)
            {
                double tempTax = Math.Ceiling((cartItem.Price * .10) * 20) / 20;
                double tempImportTax = Math.Ceiling((cartItem.Price * .05) * 20) / 20;

                // If Not Taxable and Not Imported
                if ((cartItem.Category == "Food" && !cartItem.Description.Contains("Imported")) || (cartItem.Category == "Book" && !cartItem.Description.Contains("Imported")) || (cartItem.Category == "Medical") && !cartItem.Description.Contains("Imported"))
                {
                    //salesTax += 0;
                    //importedSalesTax += 0;

                    itemSalesTax = 0;
                    cartTotalSalesTax += itemSalesTax;
                }
                // If Not Taxable and Imported
                else if ((cartItem.Category == "Food" && cartItem.Description.Contains("Imported")) || (cartItem.Category.ToLower() == "Book" && cartItem.Description.Contains("Imported")) || (cartItem.Category.ToLower() == "Medical") && cartItem.Description.Contains("Imported"))
                {
                    //salesTax += 0;
                    //importedSalesTax += tempImportTax;

                    if (cartItem.Quantity > 1)
                    {
                        itemSalesTax = tempImportTax;
                        cartTotalSalesTax += cartItem.Quantity * itemSalesTax;
                    }
                    else
                    {
                        itemSalesTax = tempImportTax;
                        cartTotalSalesTax += itemSalesTax;
                    }

                }
                // If Taxable and Not Imported
                else if ((cartItem.Category != "Food" && !cartItem.Description.Contains("Imported")) || (cartItem.Category != "Book" && !cartItem.Description.Contains("Imported")) || (cartItem.Category != "Medical") && !cartItem.Description.Contains("Imported"))
                {
                    //salesTax += tempTax;
                    //importedSalesTax += 0;

                    if (cartItem.Quantity > 1)
                    {
                        itemSalesTax = tempTax;
                        cartTotalSalesTax += cartItem.Quantity * itemSalesTax;
                    }
                    else
                    {
                        itemSalesTax = tempTax;
                        cartTotalSalesTax += itemSalesTax;
                    }

                }
                //If Taxable and Imported 
                else if ((cartItem.Category != "Food" && cartItem.Description.Contains("Imported")) || (cartItem.Category != "Book" && cartItem.Description.Contains("Imported")) || (cartItem.Category != "Medical") && cartItem.Description.Contains("Imported"))
                {
                    //salesTax += tempTax;
                    //importedSalesTax += tempImportTax;

                    if (cartItem.Quantity > 1)
                    {
                        itemSalesTax = tempTax + tempImportTax;
                        cartTotalSalesTax += cartItem.Quantity * itemSalesTax;
                    }
                    else
                    {
                        itemSalesTax = tempTax + tempImportTax;
                        cartTotalSalesTax += itemSalesTax;
                    }

                }

                // Adds to overall cart total
                cartSubTotal += (cartItem.Quantity * (cartItem.Price + itemSalesTax));

                // Item Details for total
                double tempItemTotalTax = (Math.Round((itemSalesTax) * 20)) / 20;
                double tempItemSubTotal = cartItem.Price + tempItemTotalTax;
                double tempSubTotal = Math.Round(((cartItem.Quantity * cartItem.Price) + tempItemTotalTax) * 20) / 20;

                // if item already exists in cart, print adjusted item label
                if (cartList.Contains(cartItem) && cartItem.Quantity > 1)
                {
                    Console.WriteLine(String.Format($"{cartItem.Description}: {(tempItemSubTotal * cartItem.Quantity):c} ({cartItem.Quantity} @ {tempItemSubTotal:c})"));
                }
                // if new item added to cart; Print item label
                else
                {
                    Console.WriteLine(String.Format($"{cartItem.Description}: {tempItemSubTotal:c}"));

                }

            }
           
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"Sales Tax: {cartTotalSalesTax:c}");
            Console.WriteLine($"Total: {cartSubTotal:c}");

            PaymentOptions(cartTotalSalesTax, cartSubTotal);
        }


        private static void PaymentOptions(double cartSubTotal, double cartTotalSalesTax)
        {
            bool valid = false;
            do
            {
                Console.Write("\nHow will you be paying today? Cash, Check or Credit? ");
                string userPayment = Console.ReadLine();

                if (userPayment.ToLower() == "cash")
                {
                    Cash userCash = new Cash();
                   
                    userCash = userCash.TakeCash(userCash, cartSubTotal, cartTotalSalesTax);
                    userCash.DisplayCashReceipt(cartSubTotal, cartTotalSalesTax, userCash);
                    valid = true;
                }
                else if (userPayment.ToLower() == "check")
                {
                    Check userCheck = new Check();
                   
                    userCheck = userCheck.TakeCheck(userCheck);
                    userCheck.DisplayCheckReceipt(cartSubTotal, cartTotalSalesTax, userCheck);
                    valid = true;
                }
                else if (userPayment.ToLower() == "credit")
                {
                    Credit userCredit = new Credit();
                   
                    userCredit = userCredit.TakeCredit(userCredit);
                    userCredit.DisplayCreditReceipt(cartSubTotal, cartTotalSalesTax, userCredit);
                    valid = true;
                }
                else
                {
                    Console.WriteLine("Please enter a valid response.");
                    valid = false;
                }
            }
            while (!valid);
        }

        private static void PrintHeader()
        {
            string dName = "Name";
            string dCategory = "Category";
            string dDescription = "Description";
            string dPrice = "Price";
            string dQuantity = "Quantity";


            Console.WriteLine("***Welcome to EZ Market POS***\n");
            Console.WriteLine($"{dName,-30} {dCategory,-15} {dDescription,-35} {dPrice,-15} {dQuantity,15}");
            Console.WriteLine("====================================================================================================================\n");
        }

        private static void AddItemToCart(List<Product> productList, List<Product> cartList)
        {
            bool repeat = true;
            while (repeat)
            {

                Console.WriteLine("\nWhat item would you like to have added to your cart?");
                string userInput = ValidateUserInput(Console.ReadLine().ToLower().Trim());

                foreach (Product item in productList)
                {
                    //If it contains the description of the item and price point
                    if (userInput.Contains(item.Description.ToLower()) && userInput.Contains(item.Price.ToString()))
                    {
                        string tempQty = ValidateQuantity(userInput.Substring(0, 2));
                        item.Quantity = Convert.ToInt32(tempQty);

                        if (cartList.Contains(item))
                        {
                            item.Quantity++;
                        }
                        else
                        {
                            cartList.Add(item);

                        }

                        bool confirmation = true;
                        while (confirmation)
                        {
                            Console.WriteLine("\nWould you like to add another item to your cart? (enter y or n)");
                            string userResponse = Console.ReadLine().ToLower();
                            if (userResponse == "y" || userResponse == "yes")
                            {
                                repeat = true;
                                confirmation = false;

                                Console.Clear();
                                PrintHeader();
                                PrintInventory(productList);
                            }
                            else if (userResponse == "n" || userResponse == "no")
                            {
                                repeat = false;
                                confirmation = false;
                                Console.Clear();

                            }
                            else
                            {
                                Console.WriteLine("Invalid response. Please try again...");

                            }

                        }

                        break;

                    }
                    //If it contains the Name of the Item
                    else if (userInput.Contains(item.Name.ToLower()))
                    {
                        Console.WriteLine($"\nYou've selected {item.Name}, it will be added to your cart.");
                        Console.WriteLine($"How many would you like to add to your cart?");
                        item.Quantity = Int32.Parse(Console.ReadLine());
                        cartList.Add(item);

                        bool confirmation = true;
                        while (confirmation)
                        {
                            Console.WriteLine("\nWould you like to add another item to your cart? (enter y or n)");
                            string userResponse = Console.ReadLine().ToLower();
                            if (userResponse == "y" || userResponse == "yes")
                            {
                                repeat = true;
                                confirmation = false;

                                Console.Clear();
                                PrintHeader();
                                PrintInventory(productList);
                            }
                            else if (userResponse == "n" || userResponse == "no")
                            {
                                repeat = false;
                                confirmation = false;
                                Console.Clear();

                            }
                            else
                            {
                                Console.WriteLine("Invalid response. Please try again...");

                            }

                        }

                    }
                }
            }
        }

        private static void PrintInventory(List<Product> productList)
        {
            foreach (Product item in productList)
            {
                item.PrintList(productList);
            }
        }

        #region Validation Helpers
        private static string ValidateUserInput(string userInput)
        {
            // Validates userInput
            if (!Regex.IsMatch(userInput, @"^[a-zA-z 0123456789.$]{1,150}$"))
            {
                Console.WriteLine("\nInvalid entry. Please try again...");

            }

            return userInput;
        }

        private static string ValidateQuantity(string quantity)
        {
            // Validates userInput
            if (!Regex.IsMatch(quantity, @"^(\d{1,2})"))
            {
                Console.WriteLine("\nPlease try again...");
                throw new Exception("Invalid quantity. Please try again");
            }

            return quantity;
        }

        #endregion
    }
}
