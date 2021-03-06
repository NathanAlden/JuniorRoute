﻿using System.Collections.Generic;

using Junior.Common;
using Junior.Route.Assets.FileSystem;
using Junior.Route.Common;

namespace Junior.Route.AutoRouting
{
	public class BundleRoute
	{
		private readonly IComparer<AssetFile> _assetOrder;
		private readonly Bundle _bundle;
		private readonly IAssetConcatenator _concatenator;
		private readonly string _relativePath;
		private readonly string _routeName;
		private readonly Scheme _scheme;
		private readonly IEnumerable<IAssetTransformer> _transformers;

		public BundleRoute(Bundle bundle, string routeName, Scheme scheme, string relativePath, IComparer<AssetFile> assetOrder, IAssetConcatenator concatenator, IEnumerable<IAssetTransformer> transformers)
		{
			bundle.ThrowIfNull("bundle");
			routeName.ThrowIfNull("routeName");
			relativePath.ThrowIfNull("relativePath");
			assetOrder.ThrowIfNull("assetOrder");
			concatenator.ThrowIfNull("concatenator");
			transformers.ThrowIfNull("transformers");

			_bundle = bundle;
			_routeName = routeName;
			_scheme = scheme;
			_relativePath = relativePath;
			_assetOrder = assetOrder;
			_concatenator = concatenator;
			_transformers = transformers;
		}

		public BundleRoute(Bundle bundle, string routeName, Scheme scheme, string relativePath, IComparer<AssetFile> assetOrder, IAssetConcatenator concatenator, params IAssetTransformer[] transformers)
			: this(bundle, routeName, scheme, relativePath, assetOrder, concatenator, (IEnumerable<IAssetTransformer>)transformers)
		{
		}

		public BundleRoute(Bundle bundle, string routeName, Scheme scheme, string relativePath, IAssetConcatenator concatenator, IEnumerable<IAssetTransformer> transformers)
		{
			bundle.ThrowIfNull("bundle");
			routeName.ThrowIfNull("routeName");
			relativePath.ThrowIfNull("relativePath");
			concatenator.ThrowIfNull("concatenator");
			transformers.ThrowIfNull("transformers");

			_bundle = bundle;
			_routeName = routeName;
			_scheme = scheme;
			_relativePath = relativePath;
			_concatenator = concatenator;
			_transformers = transformers;
		}

		public BundleRoute(Bundle bundle, string routeName, Scheme scheme, string relativePath, IAssetConcatenator concatenator, params IAssetTransformer[] transformers)
			: this(bundle, routeName, scheme, relativePath, concatenator, (IEnumerable<IAssetTransformer>)transformers)
		{
		}

		internal IComparer<AssetFile> AssetOrder
		{
			get
			{
				return _assetOrder;
			}
		}

		internal Bundle Bundle
		{
			get
			{
				return _bundle;
			}
		}

		internal IAssetConcatenator Concatenator
		{
			get
			{
				return _concatenator;
			}
		}

		internal Scheme Scheme
		{
			get
			{
				return _scheme;
			}
		}

		internal string RelativePath
		{
			get
			{
				return _relativePath;
			}
		}

		internal string RouteName
		{
			get
			{
				return _routeName;
			}
		}

		internal IEnumerable<IAssetTransformer> Transformers
		{
			get
			{
				return _transformers;
			}
		}
	}
}