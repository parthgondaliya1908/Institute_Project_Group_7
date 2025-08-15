import { useRef } from "react";

import "@/styles/glass-panel.css";

type GlassPanelProps = {
	children: React.ReactNode;
	className?: string;
	container?: boolean;
};

export function GlassPanel(props: GlassPanelProps) {
	const panelRef = useRef<HTMLDivElement>(null);

	function handleMouseMove(e: React.MouseEvent) {
		if (!panelRef.current || !props.container) return;

		const rect = panelRef.current.getBoundingClientRect();
		const centerX = rect.left + rect.width / 2;
		const centerY = rect.top + rect.height / 2;

		const mouseX = e.clientX - centerX;
		const mouseY = e.clientY - centerY;

		const tiltX = (mouseY / (rect.height / 2)) * -4;
		const tiltY = (mouseX / (rect.width / 2)) * 4;

		const transform = `rotateX(${tiltX}deg) rotateY(${tiltY}deg)`;

		panelRef.current.style.transform = transform;
	}

	function handleMouseLeave() {
		if (!panelRef.current || !props.container) return;

		const resetTransform = "rotateX(0deg) rotateY(0deg)";
		panelRef.current.style.transform = resetTransform;
	}

	return (
		<div className="glass-container h-full w-full" style={{ height: "100%" }}>
			<div
				ref={panelRef}
				className={`glass-panel h-full w-full ${props.className}`}
				style={{
					height: "100%",
					display: "grid",
					placeContent: "center",
				}}
				onMouseMove={handleMouseMove}
				onMouseLeave={handleMouseLeave}
			>
				{props.children}
			</div>
		</div>
	);
}
