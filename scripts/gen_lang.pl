#!/bin/perl

use strict;
use warnings;

my $content = qq(
	"craftinginfo-stainingwood-title": "Game Mechanic: Staining wood",
	"craftinginfo-stainingwood-text": "<strong>Staining wood</strong><br><i>Stain wood in the world to get the perfect color for your builds!</i><br><br>To begin staining wood, you must craft a <a href=\\\"handbooksearch://stain brush\\\">stain brush</a> and create some <a href=\\\"handbooksearch://dye\\\">dye</a>. With a uncolored wet or dry stain brush, Right mouse a bucket of dye or place the uncolored brush into a barrel of dye. After the brush has been dyed, Right mouse wooden planks, stairs, or slabs to stain them. To clean a brush, Right mouse a bucket of clean water or place the used brush into a barrel of clean water. Stain brushes must be clean before dye for staining can be applied again.<br><br>Do note that wet brushes dry out over time!",

	"item-stainbrush-blue-wet": "Stain brush (blue, wet)",
	"item-stainbrush-red-wet": "Stain brush (red, wet)",
	"item-stainbrush-yellow-wet": "Stain brush (yellow, wet)",
	"item-stainbrush-purple-wet": "Stain brush (purple, wet)",
	"item-stainbrush-pink-wet": "Stain brush (pink, wet)",
	"item-stainbrush-green-wet": "Stain brush (green, wet)",
	"item-stainbrush-orange-wet": "Stain brush (orange, wet)",
	"item-stainbrush-brown-wet": "Stain brush (brown, wet)",
	"item-stainbrush-gray-wet": "Stain brush (gray, wet)",
	"item-stainbrush-black-wet": "Stain brush (black, wet)",
	"item-stainbrush-white-wet": "Stain brush (white, wet)",
	"item-stainbrush-none-wet": "Stain brush (wet)",

	"item-stainbrush-blue-dry": "Stain brush (blue, dry)",
	"item-stainbrush-red-dry": "Stain brush (red, dry)",
	"item-stainbrush-yellow-dry": "Stain brush (yellow, dry)",
	"item-stainbrush-purple-dry": "Stain brush (purple, dry)",
	"item-stainbrush-pink-dry": "Stain brush (pink, dry)",
	"item-stainbrush-green-dry": "Stain brush (green, dry)",
	"item-stainbrush-orange-dry": "Stain brush (orange, dry)",
	"item-stainbrush-brown-dry": "Stain brush (brown, dry)",
	"item-stainbrush-gray-dry": "Stain brush (gray, dry)",
	"item-stainbrush-black-dry": "Stain brush (black, dry)",
	"item-stainbrush-white-dry": "Stain brush (white, dry)",
	"item-stainbrush-none-dry": "Stain brush (dry)",
);

my %woodtypes = (
	acacia => 'Acacia',
	aged => 'Aged',
	agedebony => 'Aged ebony',
	alder => 'Alder',
	baldcypress => 'Bald cypress',
	bearnut => 'Bearnut',
	beech => 'Beech',
	birch => 'Birch',
	blackpoplar => 'Black poplar',
	brideinwhite => 'Bride',
	catalpa => 'Catalpa',
	cedar => 'Cedar',
	douglasfir => 'Douglas fir',
	ebony => 'Ebony',
	elm => 'Elm',
	eucalyptus => 'Eucalyptus',
	honeylocust => 'Honey locust',
	kapok => 'Kapok',
	larch => 'Larch',
	mahogany => 'Mahogany',
	maple => 'Maple',
	oak => 'Oak',
	pine => 'Pine',
	purpleheart => 'Purpleheart',
	pyramidalpoplar => 'Pyramidal poplar',
	redwood => 'Redwood',
	rottenebony => 'Rotten ebony',
	sal => 'Sal',
	saxaul => 'Saxaul',
	spruce => 'Spruce',
	sycamore => 'Sycamore',
	walnut => 'Walnut',
	willow => 'Willow',
	tuja => 'Tuja',
	redcedar => 'Red cedar', 
	yew => 'Yew', 
	kauri => 'Kauri', 
	ginkgo => 'Ginko', 
	dalbergia => 'Dalbergia', 
	umnini => 'Umnini', 
	banyan => 'Banyan', 
	poplar => 'Poplar', 
	gaujacum => 'Gaujacum', 
	ghostgum => 'Ghost gum', 
	ohia => 'Ohia', 
	satinash => 'Satinash', 
	bluemahoe => 'Blue mahoe', 
	jacaranda => 'Jacaranda', 
	empresstree => 'Empress tree', 
	chlorociboria => 'Chlorociboria', 
	petrified => 'Petrified'
);

my @colors = (
	"blue",
	"red",
	"yellow",
	"purple",
	"pink",
	"green",
	"orange",
	"brown",
	"gray",
	"black",
	"white"
);

printf "{\n";

printf "$content";

printf "\n";

foreach my $woodtype (keys %woodtypes) {
	foreach my $color (@colors) {
		my $type = $woodtypes{$woodtype};
		my $en = $woodtypes{$woodtype};
		printf "\t\"block-stainedplanks-${color}-${type}\": \"${en} stained planks (${color})\",\n";
		printf "\t\"block-stainedplankslab-${color}-${type}-*\": \"${en} stained slab (${color})\",\n";
		printf "\t\"block-stainedplankstairs-${color}-${type}-*\": \"${en} stained stairs (${color})\",\n";
	}	
	printf "\n";
}

printf "\n}";
