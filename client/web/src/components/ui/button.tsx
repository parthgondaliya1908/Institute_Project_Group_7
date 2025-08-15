import { Slot } from "@radix-ui/react-slot";
import { cva, type VariantProps } from "class-variance-authority";

import { cn } from "@/lib/utils";

const baseButton = `cursor-pointer 
  active:scale-90 
  will-change-transform 
  transition duration-500 ease-in-out
  backdrop-blur-[12px]
`;

const buttonVariants = cva(
	"inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-all disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg:not([class*='size-'])]:size-4 shrink-0 [&_svg]:shrink-0 outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive",
	{
		variants: {
			variant: {
				default: cn(
					baseButton,`
            text-foreground 
            bg-white/20 dark:bg-white/10 
            hover:bg-white/30 dark:hover:bg-white/20 
            border border-white/40 dark:border-white/30
            shadow-[0_4px_30px_rgba(0,0,0,0.1)] 
          `,
        ),
				success: cn(
					baseButton, `
            text-white 
            bg-green-200 dark:bg-green-500/70
            hover:bg-green-500/85 dark:hover:dark:bg-green-500/85
            border border-green-400/50 dark:border-green-500
            shadow-[0_4px_30px_rgba(0,0,0,0.1)] 
          `,
				),
				warning: cn(
					baseButton, `
            text-white 
            bg-amber-200 dark:bg-amber-500/70
            hover:bg-amber-500/85 dark:hover:dark:bg-amber-500/85
            border border-amber-400/50 dark:border-amber-500
            shadow-[0_4px_30px_rgba(0,0,0,0.1)] 
          `,
				),
				danger: cn(
					baseButton, `
            text-white 
            bg-red-200 dark:bg-red-500/70
            hover:bg-red-500/85 dark:hover:dark:bg-red-500/85
            border border-red-400/50 dark:border-red-500
            shadow-[0_4px_30px_rgba(0,0,0,0.1)] 
          `,
				),
				ghost: cn(
					baseButton, `
            text-foreground
            hover:text-accent-foreground 
            hover:bg-accent/50 dark:hover:bg-accent/20
          `,
				),
				link: cn(baseButton, "text-primary underline-offset-4 hover:underline"),
			},
			size: {
				default: "h-10 px-4 py-2 has-[>svg]:px-3",
				sm: "h-9 rounded-md gap-1.5 px-3 has-[>svg]:px-2.5",
				lg: "h-11 rounded-md px-6 has-[>svg]:px-4",
				icon: "size-13 rounded-full",
			},
		},
		defaultVariants: {
			variant: "default",
			size: "default",
		},
	},
);

export function Button({
	className,
	variant,
	size,
	asChild = false,
	...props
}: React.ComponentProps<"button"> &
	VariantProps<typeof buttonVariants> & {
		asChild?: boolean;
	}) {
	const Comp = asChild ? Slot : "button";

	return (
		<Comp
			data-slot="button"
			className={cn(buttonVariants({ variant, size, className }))}
			{...props}
		/>
	);
}
