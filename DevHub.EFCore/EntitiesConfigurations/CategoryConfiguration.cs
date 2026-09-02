using Data.Layer.EFCore.MockData;
using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Data.Layer.EFCore.EntitiesConfigurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        int categoriesCounter = 0;
        builder.HasData(MockDB.categories.Select(c => new Category{
            Id = ++categoriesCounter,
            Name = c
        }).ToArray());
    }
}
/*
// Categories for Tech Blog
// Categories for Tech Blog
List<string> categories = new List<string>
{
// Core Development
"Artificial Intelligence",
"Machine Learning",
"Software Development",
"Programming Languages",
"Web Development",
"Mobile Development",

// Platforms & Infrastructure
"Cloud Computing",
"Networking",
"Infrastructure",
"DevOps",
"Automation",
"Open Source",

// Security & Privacy
"Cybersecurity",
"Data Privacy",
"Tech Ethics",

// Data & Emerging Tech
"Data Science",
"Analytics",
"Blockchain",
"Cryptocurrencies",
"AR/VR",
"Quantum Computing",
"Edge Computing",

// Hardware & Devices
"Hardware",
"Gadgets",
"IoT",
"Wearables",
"Smart Home",

// Industry & Trends
"Tech Industry News",
"Tech Reviews",
"Tech Conferences",
"Tech Startups",
"Entrepreneurship",
"Tech Policy",
"Future of Work",
"Green Tech",
"Sustainability",

// Careers & Productivity
"Tech Careers",
"Education",
"Productivity Tools",
"Workflows"
};
// Tags for Tech Blog (expanded with many programming languages)
List<string> tags = new List<string>
{
// AI & Emerging Tech
"AI Tools", "Neural Networks", "Deep Learning", "Generative AI", "LLMs", "Prompt Engineering",
"Quantum Computing", "Edge Computing", "5G Technology", "Metaverse", "Virtual Reality",
"Augmented Reality", "Mixed Reality",

// Programming Languages
"Python", "JavaScript", "TypeScript", "Java", "C#", "C++", "C", "GoLang", "Rust", "Ruby",
"PHP", "Swift", "Kotlin", "Objective-C", "Perl", "Scala", "Haskell", "Elixir", "Erlang",
"Dart", "R", "MATLAB", "Julia", "Shell Scripting", "PowerShell", "SQL", "NoSQL",
"Fortran", "COBOL", "Assembly", "F#", "Visual Basic", "Groovy", "Lua",

// Web & Mobile
"React", "Angular", "Vue.js", "Node.js", "Svelte", "Next.js", "Nuxt.js",
"iOS Development", "Android Development", "Flutter", "React Native",

// Cloud & DevOps
"AWS", "Azure", "Google Cloud", "SaaS", "Microservices", "API Development",
"Containerization", "Kubernetes", "CI/CD Pipelines", "Serverless Computing",
"Cloud-Native Apps",

// Cybersecurity
"Cybersecurity Tips", "Ethical Hacking", "Penetration Testing", "Zero Trust Security",
"Cybersecurity Frameworks", "Data Privacy", "Tech Ethics",

// Data & Analytics
"Big Data", "Data Visualization", "Machine Learning Models", "Data Science",
"Blockchain Basics", "Smart Contracts", "NFTs",

// Productivity & Careers
"Productivity Hacks", "Remote Work Tools", "Coding Bootcamps", "Tech Career Advice",
"Tech Recruiting", "Digital Transformation", "Future of Work",

// Hardware & Gadgets
"Gadgets Review", "Wearable Tech", "Smart Home Devices", "IoT Devices",

// Community & Trends
"Open Source", "Tech Startups", "Tech Conferences", "Tech Innovations",
"Green Tech", "Sustainable Computing", "Tech Policy", "Tech Trends"
};
*/
/*
// Categories with suggested color codes
Dictionary<string, string> categoryColors = new Dictionary<string, string>
{
{ "Artificial Intelligence", "#4B0082" },   // Deep Indigo
{ "Machine Learning", "#8A2BE2" },          // Blue Violet
{ "Software Development", "#1E90FF" },      // Dodger Blue
{ "Programming Languages", "#4682B4" },     // Steel Blue
{ "Web Development", "#00CED1" },           // Dark Turquoise
{ "Mobile Development", "#20B2AA" },        // Light Sea Green

{ "Cloud Computing", "#6495ED" },           // Cornflower Blue
{ "Networking", "#2E8B57" },                // Sea Green
{ "Infrastructure", "#708090" },            // Slate Gray
{ "DevOps", "#FF8C00" },                    // Dark Orange
{ "Automation", "#FFD700" },                // Gold
{ "Open Source", "#32CD32" },               // Lime Green

{ "Cybersecurity", "#DC143C" },             // Crimson
{ "Data Privacy", "#B22222" },              // Firebrick
{ "Tech Ethics", "#A0522D" },               // Sienna

{ "Data Science", "#00BFFF" },              // Deep Sky Blue
{ "Analytics", "#40E0D0" },                 // Turquoise
{ "Blockchain", "#9932CC" },                // Dark Orchid
{ "Cryptocurrencies", "#FF4500" },          // Orange Red
{ "AR/VR", "#FF69B4" },                     // Hot Pink
{ "Quantum Computing", "#6A5ACD" },         // Slate Blue
{ "Edge Computing", "#191970" },            // Midnight Blue

{ "Hardware", "#8B4513" },                  // Saddle Brown
{ "Gadgets", "#DAA520" },                   // Goldenrod
{ "IoT", "#00FA9A" },                       // Medium Spring Green
{ "Wearables", "#FF1493" },                 // Deep Pink
{ "Smart Home", "#7FFF00" },                // Chartreuse

{ "Tech Industry News", "#000080" },        // Navy
{ "Tech Reviews", "#556B2F" },              // Dark Olive Green
{ "Tech Conferences", "#483D8B" },          // Dark Slate Blue
{ "Tech Startups", "#FF6347" },             // Tomato
{ "Entrepreneurship", "#CD5C5C" },          // Indian Red
{ "Tech Policy", "#2F4F4F" },               // Dark Slate Gray
{ "Future of Work", "#4682B4" },            // Steel Blue
{ "Green Tech", "#228B22" },                // Forest Green
{ "Sustainability", "#006400" },            // Dark Green

{ "Tech Careers", "#8B0000" },              // Dark Red
{ "Education", "#0000CD" },                 // Medium Blue
{ "Productivity Tools", "#FFB6C1" },        // Light Pink
{ "Workflows", "#00FF7F" }                  // Spring Green
};
*/