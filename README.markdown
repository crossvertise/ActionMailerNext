Introduction
============
ActionMailer.Net aims to be an easy, and relatively painless way to send email from your ASP.NET MVC application.  The concept is pretty simple.  We render HTML by utilizing some pretty snazzy view engines, so why can't we do the same thing for email?


Installation
============
Right now you'll have to check the downloads section on bitbucket, or grab the source and build it yourself.  Once done, you just have to reference the **ActionMailer.Net.dll** within your project and you're ready to go!

I know NuGet is out there, and once this project stabilizes a bit, I'll be happy to add it.  I'm hoping to get a few early adopters to be my ever-so-helpful guinea pigs.


Usage
=====
This is the easy part, I swear!  There are three steps on the way to email awesomeness:

 1. Create a new controller in your project.  I called mine **MailController**, but any name will do.  Inside your controller, each action will need to return **EmailResult** objects.  The best way to do that is to use the handy **Email()** method.  Here's some sample code to get you started.

        public class MailController : MailerBase
        {
            public EmailResult VerificationEmail(User model)
            {
                To.Add(model.EmailAddress);
                From = "no-reply@mycoolsite.com";
                Subject = "Welcome to My Cool Site!";
                return Email(model);
            }
        }

 2. Now we need to create a **View** for this email.  The view can use any ViewEngine you like, and it will even work with master pages (or layouts in Razor).  The views live in the same place your normal views do.  Here's a sample view that matches the nifty email action above.

        @model User

        @{
            Layout = null;
        }

        Welcome to My Cool Site, @User.FirstName

        We need you to verify your email.  Click this nifty link to get verified!

        @Url.Action("Verify", "Account", new { code = @User.EmailActivationToken.ToString() })

        Thanks!

 3. Now it's just a matter of calling your action directly any time you need to send an email.  You can call it like this:

        var newUser = _myRepository.CreateUser(model);
        new MailController().VerificationEmail(newUser).Deliver();


Advanced Stuff
==============
The astute reader will notice that **MailerBase** has a couple of events:  **OnMailSent** and **OnMailSending**.  Feel free to have fun with those :)

You might also notice that there is a **DeliverAsync()** method.  I don't offer a callback here because even with Async delivery, the **OnMailSent** event will fire after the send operation completes.


Notice
======
Please realize that this library is a *super-early alpha* and I'd love some help!  Feel free to submit pull requests or patches.


To-Do
=====
This is just a short list of things that are still left to do.  I will make my way through this list eventually, but your help is appreciated.

  - **Attachments**:  I really want to add more than just simple attachments.  I'd like the ability to do inline attachments as well.  This needs to be done with some HtmlHelper extension methods, probably.  Still brainstorming.
  - **Unit Tests**:  Right now I don't have any unit tests complete... I know this is bad.  I want to add them in, but I was too excited to get this thing out there.  I definitely will go back and put some tests around everything.