﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Web;

using Junior.Common;
using Junior.Route.Routing.RequestValueComparers;

namespace Junior.Route.Routing.Restrictions
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class UrlAuthorityRestriction : IRestriction, IEquatable<UrlAuthorityRestriction>
	{
		private readonly string _authority;
		private readonly IRequestValueComparer _comparer;

		public UrlAuthorityRestriction(string authority, IRequestValueComparer comparer)
		{
			comparer.ThrowIfNull("comparer");
			authority.ThrowIfNull("authority");

			_authority = authority;
			_comparer = comparer;
		}

		public string Authority
		{
			get
			{
				return _authority;
			}
		}

		public IRequestValueComparer Comparer
		{
			get
			{
				return _comparer;
			}
		}

		// ReSharper disable UnusedMember.Local
		private string DebuggerDisplay
			// ReSharper restore UnusedMember.Local
		{
			get
			{
				return _authority;
			}
		}

		public bool Equals(UrlAuthorityRestriction other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return String.Equals(_authority, other._authority) && Equals(_comparer, other._comparer);
		}

		public MatchResult MatchesRequest(HttpRequestBase request)
		{
			request.ThrowIfNull("request");

			return String.Equals(_authority, request.Url.Authority) ? MatchResult.RestrictionMatched(this.ToEnumerable()) : MatchResult.RestrictionNotMatched(Enumerable.Empty<IRestriction>(), this.ToEnumerable());
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((UrlAuthorityRestriction)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((_authority != null ? _authority.GetHashCode() : 0) * 397) ^ (_comparer != null ? _comparer.GetHashCode() : 0);
			}
		}
	}
}