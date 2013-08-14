﻿using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

using Junior.Common;
using Junior.Route.AutoRouting.ParameterMappers;
using Junior.Route.AutoRouting.ParameterMappers.ModelPropertyMappers.Request;

using NUnit.Framework;

using Rhino.Mocks;

namespace Junior.Route.UnitTests.AutoRouting.ParameterMappers.ModelPropertyMappers.Request
{
	public static class RequestMapperTester
	{
		private class ExceptionMapper : RequestMapper
		{
			public ExceptionMapper(bool caseSensitive = false, DataConversionErrorHandling errorHandling = DataConversionErrorHandling.UseDefaultValue)
				: base(NameValueCollectionSource.Form, caseSensitive, errorHandling)
			{
			}

			public override Task<bool> CanMapTypeAsync(HttpContextBase context, Type propertyType)
			{
				return true.AsCompletedTask();
			}

			protected override Task<MapResult> OnMapAsync(HttpContextBase context, string value, Type propertyType)
			{
				throw new ApplicationException();
			}
		}

		private class Mapper : RequestMapper
		{
			public Mapper(NameValueCollectionSource source, bool caseSensitive = false, DataConversionErrorHandling errorHandling = DataConversionErrorHandling.UseDefaultValue)
				: base(source, caseSensitive, errorHandling)
			{
			}

			public override Task<bool> CanMapTypeAsync(HttpContextBase context, Type propertyType)
			{
				return true.AsCompletedTask();
			}

			protected override Task<MapResult> OnMapAsync(HttpContextBase context, string value, Type propertyType)
			{
				return MapResult.ValueMapped(value).AsCompletedTask();
			}
		}

		[TestFixture]
		public class When_conversion_fails_and_configured_to_throw_exception
		{
			[SetUp]
			public void SetUp()
			{
				_mapper = new ExceptionMapper(errorHandling:DataConversionErrorHandling.ThrowException);
				_request = MockRepository.GenerateMock<HttpRequestBase>();
				_request.Stub(arg => arg.Form).Return(new NameValueCollection { { "I", "1.2" } });
				_context = MockRepository.GenerateMock<HttpContextBase>();
				_context.Stub(arg => arg.Request).Return(_request);
			}

			private RequestMapper _mapper;
			private HttpRequestBase _request;
			private HttpContextBase _context;

			public class Model
			{
				public int I
				{
					get;
					set;
				}
			}

			[Test]
			[TestCase(typeof(Model), "I")]
			[ExpectedException(typeof(ApplicationException))]
#warning Update to use async Assert.That(..., Throws.InstanceOf<>) when NUnit 2.6.3 becomes available
			public async void Must_throw_exception(Type type, string propertyName)
			{
				PropertyInfo propertyInfo = type.GetProperty(propertyName);

				await _mapper.MapAsync(_context, type, propertyInfo);
			}
		}

		[TestFixture]
		public class When_conversion_fails_and_configured_to_use_default_value
		{
			[SetUp]
			public void SetUp()
			{
				_mapper = new ExceptionMapper();
				_request = MockRepository.GenerateMock<HttpRequestBase>();
				_request.Stub(arg => arg.Form).Return(new NameValueCollection { { "I", "1.2" } });
				_context = MockRepository.GenerateMock<HttpContextBase>();
				_context.Stub(arg => arg.Request).Return(_request);
			}

			private ExceptionMapper _mapper;
			private HttpRequestBase _request;
			private HttpContextBase _context;

			public class Model
			{
				public int i
				{
					get;
					set;
				}
			}

			[Test]
			[TestCase(typeof(Model), "i", 0)]
			public async void Must_map_default_value_to_properties(Type type, string propertyName, object expectedValue)
			{
				PropertyInfo propertyInfo = type.GetProperty(propertyName);
				MapResult result = await _mapper.MapAsync(_context, type, propertyInfo);

				Assert.That(result.ResultType, Is.EqualTo(MapResultType.ValueMapped));
				Assert.That(result.Value, Is.EqualTo(expectedValue));
			}
		}

		[TestFixture]
		public class When_mapping_from_source
		{
			public class Model
			{
				public int i
				{
					get;
					set;
				}
			}

			[Test]
			[TestCase(typeof(Model), NameValueCollectionSource.Form, "i", "1.2")]
			[TestCase(typeof(Model), NameValueCollectionSource.QueryString, "i", "2.2")]
			public async void Must_map_to_properties_from_specified_source(Type type, NameValueCollectionSource source, string propertyName, object expectedValue)
			{
				var mapper = new Mapper(source);
				var request = MockRepository.GenerateMock<HttpRequestBase>();
				var context = MockRepository.GenerateMock<HttpContextBase>();

				request.Stub(arg => arg.Form).Return(new NameValueCollection { { "I", "1.2" } });
				request.Stub(arg => arg.QueryString).Return(new NameValueCollection { { "I", "2.2" } });
				context.Stub(arg => arg.Request).Return(request);

				PropertyInfo propertyInfo = type.GetProperty(propertyName);
				MapResult result = await mapper.MapAsync(context, type, propertyInfo);

				Assert.That(result.ResultType, Is.EqualTo(MapResultType.ValueMapped));
				Assert.That(result.Value, Is.EqualTo(expectedValue));
			}
		}

		[TestFixture]
		public class When_performing_case_insensitive_mapping
		{
			[SetUp]
			public void SetUp()
			{
				_mapper = new Mapper(NameValueCollectionSource.Form);
				_request = MockRepository.GenerateMock<HttpRequestBase>();
				_request.Stub(arg => arg.Form).Return(new NameValueCollection { { "D", "1.2" } });
				_context = MockRepository.GenerateMock<HttpContextBase>();
				_context.Stub(arg => arg.Request).Return(_request);
			}

			private Mapper _mapper;
			private HttpRequestBase _request;
			private HttpContextBase _context;

			public class Model
			{
				public double d
				{
					get;
					set;
				}
			}

			[Test]
			[TestCase(typeof(Model), "d")]
			public async void Must_map_to_properties_whose_names_differ_by_case(Type type, string propertyName)
			{
				PropertyInfo propertyInfo = type.GetProperty(propertyName);
				MapResult result = await _mapper.MapAsync(_context, type, propertyInfo);

				Assert.That(result.ResultType, Is.EqualTo(MapResultType.ValueMapped));
				Assert.That(result.Value, Is.EqualTo("1.2"));
			}
		}

		[TestFixture]
		public class When_performing_case_sensitive_mapping_from_form_values
		{
			[SetUp]
			public void SetUp()
			{
				_mapper = new Mapper(NameValueCollectionSource.Form, true);
				_request = MockRepository.GenerateMock<HttpRequestBase>();
				_request.Stub(arg => arg.Form).Return(new NameValueCollection { { "S", "value" }, { "I", "0" } });
				_context = MockRepository.GenerateMock<HttpContextBase>();
				_context.Stub(arg => arg.Request).Return(_request);
			}

			private Mapper _mapper;
			private HttpRequestBase _request;
			private HttpContextBase _context;

			public class Model1
			{
				public string S
				{
					get;
					set;
				}

				public double s
				{
					get;
					set;
				}

				public int I
				{
					get;
					set;
				}
			}

			public class Model2
			{
				public string s
				{
					get;
					set;
				}
			}

			[Test]
			[TestCase(typeof(Model1), "S", "value")]
			[TestCase(typeof(Model1), "I", "0")]
			public async void Must_map_to_properties_whose_names_have_same_case(Type type, string propertyName, object expectedValue)
			{
				PropertyInfo propertyInfo = type.GetProperty(propertyName);
				MapResult result = await _mapper.MapAsync(_context, type, propertyInfo);

				Assert.That(result.ResultType, Is.EqualTo(MapResultType.ValueMapped));
				Assert.That(result.Value, Is.EqualTo(expectedValue));
			}

			[Test]
			[TestCase(typeof(Model2), "s")]
			public async void Must_not_map_to_properties_whose_names_have_different_case(Type type, string propertyName)
			{
				PropertyInfo propertyInfo = type.GetProperty(propertyName);
				MapResult result = await _mapper.MapAsync(_context, type, propertyInfo);

				Assert.That(result.ResultType, Is.EqualTo(MapResultType.ValueNotMapped));
			}
		}
	}
}