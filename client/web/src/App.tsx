import { GlassPanel } from "@/components/containers/glass-panel";
import { Button } from "@/components/ui/button";

export function App() {
	return (
		<div className="flex items-center justify-center min-h-screen">
			<div className="h-[85svh] w-[90svw]">
				<GlassPanel container className="p-3">
					<GlassPanel className="p-3">
						<Button size="lg" variant="default">
							Button
						</Button>
					</GlassPanel>
				</GlassPanel>
			</div>
		</div>
	);
}
