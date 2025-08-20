using Allup_Core.Entities;
using Allup_DataAccess.Interceptors;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Allup_DataAccess.DAL
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        private readonly BaseEntityInterceptor _interceptor;


        public AppDbContext(DbContextOptions<AppDbContext> options, BaseEntityInterceptor interceptor) : base(options)
        {
            _interceptor = interceptor;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.AddInterceptors(_interceptor);
        }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<FeaturesBanner> FeaturesBanners { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<ReclamBanner> ReclamBanners { get; set; }
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Blog> Blogs { get; set; } = null!;
        public DbSet<BlogTag> BlogTags { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<About> Abouts { get; set; }

        //SignalR
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<AppUserChat> AppUserChats { get; set; } = null!;


        // Many to Many
        public DbSet<TagProduct> TagProducts { get; set; }
        public DbSet<SizeProduct> SizeProducts { get; set; }
        public DbSet<ColorProduct> ColorProducts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);

            //modelBuilder.Entity<Product>()
            //           .HasOne(p => p.Ingredient)
            //           .WithMany()
            //           .HasForeignKey(p => p.IngredientId)
            //           .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
