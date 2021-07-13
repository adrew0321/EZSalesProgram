using EZSalesProgram.PaymentTypes;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EZSalesProgram.Processing
{
    class Payment : Program
    {
        #region Properties
       
        public static string ReceiptFilePath = "../../../Processing/txtFiles/Receipt.txt";
        public static DateTime Date = DateTime.Now;

        #endregion



        #region Methods
        public void DisplayCheckReceipt(double subTotal, double salesTax, Check userPayment)
        {
            StreamWriter writer = new StreamWriter($"{ReceiptFilePath}", true);

            Console.Clear();

            //Display Check Receipt
            Console.WriteLine("\n\t*****Receipt*****");
            Console.WriteLine($"Date: {Date,-10}");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"Sales Tax: {subTotal:c}");
            Console.WriteLine($"Grand Total: {salesTax:c}");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"Amount Paid: {userPayment.AmountGiven:c}");
            Console.WriteLine($"Check Number: {userPayment.CheckNum}");
            Console.WriteLine("AMOUNT PAID");

            //writes Check Receipt to txt file
            writer.WriteLine("\n\t*****Receipt*****");
            writer.WriteLine($"Date: {Date,-10}");
            writer.WriteLine("-------------------------------------------------");
            writer.WriteLine($"Sales Tax: {subTotal:c}");
            writer.WriteLine($"Grand Total: {salesTax:c}");
            writer.WriteLine("-------------------------------------------------");
            writer.WriteLine($"Amount Paid: {userPayment.AmountGiven:c}");
            writer.WriteLine($"Check Number: {userPayment.CheckNum}");
            writer.WriteLine("AMOUNT PAID");
            writer.WriteLine("===========================================================");
            writer.Close();
        }

        public void DisplayCashReceipt(double subTotal, double salesTax, Cash userPayment)
        {
            StreamWriter writer = new StreamWriter($"{ReceiptFilePath}", true);

            Console.Clear();

            //Display Cash Receipt
            Console.WriteLine("\n\t*****Receipt*****");
            Console.WriteLine($"Date: {Date,-10}");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"Sales Tax: {subTotal:c}");
            Console.WriteLine($"Grand Total: {salesTax:c}");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"Amount Given: {userPayment.AmountGiven:c}");
            Console.WriteLine($"Change: {userPayment.Change:c}");

            //Writes Cash Receipt txt file
            writer.WriteLine("\n\t*****Receipt*****");
            writer.WriteLine($"Date: {Date,-10}");
            writer.WriteLine("-------------------------------------------------");
            writer.WriteLine($"Sales Tax: {subTotal:c}");
            writer.WriteLine($"Grand Total: {salesTax:c}");
            writer.WriteLine("-------------------------------------------------");
            writer.WriteLine($"Amount Given: {userPayment.AmountGiven:c}");
            writer.WriteLine($"Change: {userPayment.Change:c}");
            writer.WriteLine("===========================================================");
            writer.Close();

        }

        public void DisplayCreditReceipt(double subTotal, double salesTax, Credit userPayment)
        {
            StreamWriter writer = new StreamWriter($"{ReceiptFilePath}", true);

            Console.Clear();

            //Displays Credit Receipt
            Console.WriteLine("\n\t*****Receipt*****");
            Console.WriteLine($"Date: {Date,-10}");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"Sales Tax: {subTotal:c}");
            Console.WriteLine($"Grand Total: {salesTax:c}");
            Console.WriteLine("-------------------------------------------------");
            Console.Write("xxxx-xxxx-xxxx-");
            Console.Write(userPayment.CardNum[userPayment.CardNum.Length - 1]);
            Console.Write(userPayment.CardNum[userPayment.CardNum.Length - 2]);
            Console.Write(userPayment.CardNum[userPayment.CardNum.Length - 3]);
            Console.WriteLine(userPayment.CardNum[userPayment.CardNum.Length - 4]);
            Console.WriteLine("AMOUNT PAID");

            //Writes Credit Receipt to txt file
            writer.WriteLine("\n\t*****Receipt*****");
            writer.WriteLine($"Date: {Date,-10}");
            writer.WriteLine("-------------------------------------------------");
            writer.WriteLine($"Sales Tax: {subTotal:c}");
            writer.WriteLine($"Grand Total: {salesTax:c}");
            writer.WriteLine("-------------------------------------------------");
            writer.WriteLine(userPayment.CardNum);
            writer.WriteLine("AMOUNT PAID");
            writer.WriteLine("===========================================================");
            writer.Close();
        }

        public Cash TakeCash(Cash userCash, double cartSubTotal, double cartTotalSalesTax)
        {
            string amount;
            double validAmount;
            bool valid = true;

            do
            {
                //gather the amount they are paying with in amount
                Console.WriteLine("Please enter the amount you are paying with.");
                amount = Console.ReadLine().Trim();

                if (Convert.ToDouble(amount) < cartSubTotal)
                {
                    Console.WriteLine("Due to a recent coin shortage; we require you to use exact change when utilizing cash payments. Please try again using exact change");
                    valid = false;

                }
                else if (double.TryParse(amount, out validAmount))
                {
                    //if amount can be parsed, set the object values
                    userCash.AmountGiven = validAmount;
                    userCash.Change = userCash.GetChange(cartSubTotal + cartTotalSalesTax);
                    valid = true;
                }
                else
                {
                    //if amount cannot be parsed, tell the user to try again
                    Console.WriteLine("Please enter a valid number.");
                    valid = false;
                }
            }
            while (!valid);
            return userCash;
        }

        public Check TakeCheck(Check userCheck)
        {
            string amount;
            double validAmount;
            bool valid = true;

            do
            {
                //Ask how much the check is for and store it in amount
                Console.WriteLine("Please enter the amount the check is for.");
                amount = Console.ReadLine().Trim();

                if (double.TryParse(amount, out validAmount))
                {
                    //if amount can be parsed set the object values
                    userCheck.AmountGiven = validAmount;
                    Console.WriteLine("Please enter the check number");
                    userCheck.CheckNum = Console.ReadLine().Trim();
                    valid = true;
                }
                else
                {
                    //if amount cannot be parsed, tell user to try again
                    Console.WriteLine("Please enter a valid amount.");
                    valid = false;
                }
            }
            while (!valid);
            return userCheck;
        }

        public Credit TakeCredit(Credit userCredit)
        {
            string cardNum;
            string cardPattern = @"^[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}$";
            string expDate;
            string expPattern = @"^[0-9]{2}/[0-9]{2}$";
            string cvv;
            string cvvPattern = @"^[0-9]{3}$";
            bool valid = false;

            Regex cardRgx = new Regex(cardPattern);
            Regex expRgx = new Regex(expPattern);
            Regex cvvRgx = new Regex(cvvPattern);

            do
            {
                //ask the user for the card number, store it in cardNum
                Console.WriteLine("Please enter the card number. (xxxx-xxxx-xxxx-xxxx)");
                cardNum = Console.ReadLine().Trim();

                if (cardRgx.IsMatch(cardNum))
                {
                    //if the card number matches the pattern, set object value
                    userCredit.CardNum = cardNum;
                    valid = true;
                }
                else
                {
                    //if not, ask them to try again
                    Console.WriteLine("Please enter a valid card number.");
                    valid = false;
                }
            }
            while (!valid);

            do
            {
                //ask the user for the expiration date
                Console.WriteLine("Please enter the expiration date. (xx/xx)");
                expDate = Console.ReadLine();
                if (expRgx.IsMatch(expDate))
                {
                    //if it matches the pattern, set object value
                    userCredit.ExpDate = expDate;
                    valid = true;
                }
                else
                {
                    //if not, try again
                    Console.WriteLine("Please enter a valid expiration date.");
                    valid = false;
                }

            }
            while (!valid);

            do
            {
                //ask user for cvv
                Console.WriteLine("Please enter your card's CVV.");
                cvv = Console.ReadLine();
                if (cvvRgx.IsMatch(cvv))
                {
                    //if it matches, set object value
                    userCredit.CVV = cvv;
                    valid = true;
                }
                else
                {
                    //if not, tell em to shove it
                    Console.WriteLine("Please enter a valid CVV.");
                    valid = false;
                }
            }
            while (!valid);
            return userCredit;
        }

        #endregion
    }
}
